<script setup>
import { ref, onMounted, onBeforeUnmount, defineProps, defineEmits } from "vue";

const props = defineProps({
  url: {
    type: String,
    required: true,
  },
});

const emit = defineEmits(["updateCoordinates", "error"]);

const connection = ref(null);

onMounted(() => {

  // websocket connect
  connection.value = new WebSocket(props.url);

  // websocket callbacks
  connection.value.onmessage = (event) => {
    const receivedData = JSON.parse(event.data);
    if (receivedData.coordinates) {

      // update leaflet component with new coords
      emit("updateCoordinates", receivedData.coordinates);
    }
  };

  connection.value.onerror = (error) => {
    console.error("WebSocket error:", error);
    emit("error", error);
  };

  connection.value.onopen = () => {
    console.log("Successfully connected to the WebSocket server...");
  };
});

onBeforeUnmount(() => {
  
// websocket disconnect
  if (connection.value) {
    connection.value.close();
  }
});
</script>
