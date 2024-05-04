<template>
  <div>
    <GetSignalsButton
      @signalsFetched="handleSignalsFetched"
      @fetchError="handleFetchError"
    />
    <LeafletMap :coordinates="coordinates" :signalMarkers="signalMarkers" />
    <WebSocket
      ref="websocketRef"
      url="ws://localhost:8081/ws/coordinates"
      @updateCoordinates="updateCoordinates"
      @error="handleError"
    />
  </div>
</template>

<script setup>
import { ref } from "vue";
import LeafletMap from "./components/LeafletMap.vue";
import WebSocket from "./components/WebSocket.vue";
import GetSignalsButton from "./components/GetSignalsButton.vue";

const coordinates = ref(null);
const signalMarkers = ref(null);
const websocketRef = ref(null);
let firstRed = ref(null);
let wait = false;

//updates Leaflet's coordinates
const updateCoordinates = (coords) => {
  
  //if red signal exists
  if (firstRed.value != null) {
    
    //and not waiting, update coords
    if (!wait) coordinates.value = coords;

    //if coordinates of tram is same as first red signal, set wait true
    if (
      firstRed.value.Coordinates.Latitude == coords.latitude &&
      firstRed.value.Coordinates.Longitude == coords.longitude
    ) {
      wait = true;
    }
  }
  // if there is not red signal, update coords
  else coordinates.value = coords;
};

//callback from SignalButton component, its fetch all signals from server with random light state
const handleSignalsFetched = (signalsJsonString) => {
  try {
    const signalsData = JSON.parse(signalsJsonString);

    if (Array.isArray(signalsData.Signals)) {

      //get first signal with red light and run tram 
      firstRed.value = signalsData.Signals[signalsData.FirstRed];
      wait = false;

      // extract coordinates from signals
      const extractedCoordinates = signalsData.Signals.map((signal) => {
        return {
          latitude: signal.Coordinates.Latitude,
          longitude: signal.Coordinates.Longitude,
          state: signal.State,
          // additional data can be added here if needed (e.g., State, Name)
        };
      });
      // update signalMarkers to pass the coordinates of signals to LeafletMap
      signalMarkers.value = extractedCoordinates;
    } else {
      console.error("Expected an array of signals, but received:", signalsData);
    }
  } catch (error) {
    console.error("Error parsing JSON:", error);
  }
};
const handleError = (error) => {
  console.error("WebSocket error:", error);
};
const handleFetchError = (error) => {
  console.error("Error fetching signals:", error);
};
</script>
