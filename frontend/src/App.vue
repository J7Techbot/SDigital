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

const updateCoordinates = (coords) => {
  if (firstRed.value != null) {
    if (!wait) coordinates.value = coords;
    if (
      firstRed.value.Coordinates.Latitude == coords.latitude &&
      firstRed.value.Coordinates.Longitude == coords.longitude
    ) {
      console.log("wait");
      wait = true;
    }
  }
  // update tram coordinates in leaflet
  else coordinates.value = coords;
};
const handleSignalsFetched = (signalsJsonString) => {
  try {
    const signalsData = JSON.parse(signalsJsonString);

    if (Array.isArray(signalsData.Signals)) {
      console.log("Received signals:", signalsData.Signals);

      firstRed.value = signalsData.Signals[signalsData.FirstRed];
      console.log("firstRed:", firstRed);

      if (signalsData.FirstRed == -1) wait = true;
      else wait = false;

      // extract coordinates from signals
      const extractedCoordinates = signalsData.Signals.map((signal) => {
        return {
          latitude: signal.Coordinates.Latitude,
          longitude: signal.Coordinates.Longitude,
          state: signal.State,
          // additional data can be added here if needed (e.g., State, Name)
        };
      });
      console.log("extractedCoordinates", extractedCoordinates);
      // update signalMarkers to pass the coordinates to LeafletMap
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
