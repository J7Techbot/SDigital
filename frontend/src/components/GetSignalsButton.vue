<template>
    <div>
      <button @click="fetchSignals">Fetch Signals</button>
    </div>
  </template>
  
  <script setup>
  import axios from 'axios';
  import { defineEmits } from 'vue';
  
  const emit = defineEmits(['signalsFetched', 'fetchError']);
  
  //get signals with random state
  const fetchSignals = async () => { 
    try {
      const response = await axios.get('http://localhost:8081/getSignals');     
      emit('signalsFetched', response.data);
    } catch (error) {
      console.error('Error fetching signals:', error);
      emit('fetchError', error);
    }
  };
  </script>