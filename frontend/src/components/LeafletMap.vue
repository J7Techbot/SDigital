<template>
    <div>
      <h3>An interactive leaflet map</h3>
      <div id="map" style="height: 90vh;"></div>
    </div>
  </template>
  
  <script setup>
  import { defineProps, ref, onMounted, watch } from 'vue';
  import 'leaflet/dist/leaflet.css';
  import * as L from 'leaflet';
  import icon from './resources/tram.png';
  import signalIconRed from './resources/signalRed.png';
  import signalIconGreen from './resources/signalGreen.png';

  // define props for the component
  const props = defineProps({
      coordinates: Object,
      signalMarkers: Array 
  });
  
  const mapRef = ref(null);
  const markerRef = ref(null);
  const markersRef = ref([]); // array to hold markers
  
  // define default icon for the tram
  const tramIcon = L.icon({
      iconUrl: icon,
      iconSize: [30, 30],
      iconAnchor: [15, 15],
  });
  
  // define a custom icon for signal marker red
  const signalRed = L.icon({
      iconUrl: signalIconRed,
      iconSize: [30, 30], 
      iconAnchor: [15, 15], 
  });

  // define a custom icon for signal marker green
  const signalGreen = L.icon({
      iconUrl: signalIconGreen,
      iconSize: [30, 30], 
      iconAnchor: [15, 15], 
  });
  
  // initialize the map
  onMounted(() => {
      mapRef.value = L.map('map', { zoomControl: true }).setView([61.45151, 23.873911], 17);
      L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
          maxZoom: 20,
          attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>',
      }).addTo(mapRef.value);
  
      markerRef.value = L.marker([61.4988562, 23.7729664], { icon: tramIcon }).addTo(mapRef.value);
  });
  
  // watch for changes in coordinates and update the initial marker
  watch(
      () => props.coordinates,
      (newCoordinates) => {
          if (markerRef.value && newCoordinates) {
              markerRef.value.setLatLng([newCoordinates.latitude, newCoordinates.longitude]);
          }
      }
  );
  
  // watch for changes in signalMarkers and update map markers
  watch(
      () => props.signalMarkers,
      (newMarkers) => {
          // remove existing markers
          markersRef.value.forEach((marker) => mapRef.value.removeLayer(marker));
          markersRef.value = [];
  
          // add new markers
          if (newMarkers) {
              newMarkers.forEach((signal) => {
                let iconWithColor = signalRed
                console.log('signal.State:', signal.state);
                if(signal.state == 2){
                    iconWithColor = signalGreen
                }
                  // create a marker for each signal using the custom icon
                  const marker = L.marker([signal.latitude, signal.longitude], { icon: iconWithColor }).addTo(mapRef.value);
                  markersRef.value.push(marker);
              });
          }
      }
  );
  </script>