<template>
  <div
    v-if="visible"
    class="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-70 backdrop-blur-sm transition-opacity duration-300"
  >
    <!-- Modal Container -->
    <div
      class="relative bg-gradient-to-br from-red-600 via-red-500 to-red-400 text-white rounded-3xl w-11/12 max-w-lg mx-auto p-8 shadow-2xl transform transition-transform duration-500 hover:scale-105"
    >
      <!-- Close Button -->
      <button
        @click="hideModal"
        class="absolute top-4 right-4 text-white text-opacity-80 hover:text-opacity-100 transition duration-200"
      >
        <i class="fa-solid fa-times text-3xl"></i>
      </button>

      <!-- Modal Header -->
      <div class="text-center mb-6">
        <div
          class="bg-white bg-opacity-20 p-4 rounded-full inline-flex items-center justify-center shadow-md"
        >
          <i class="fa-solid fa-trash text-5xl text-yellow-400"></i>
        </div>
        <h3 class="text-3xl font-bold mt-4">{{ props.title }}</h3>
      </div>

      <!-- Modal Body -->
      <p class="text-center text-lg text-white text-opacity-90 leading-relaxed mb-8">
        {{ props.message }}
      </p>

      <!-- Modal Actions -->
      <div class="flex justify-around">
        <button
          @click="onConfirm"
          class="bg-red-700 hover:bg-red-600 text-white text-lg px-6 py-3 rounded-full shadow-md font-medium transition-all focus:ring focus:ring-red-300"
        >
          Yes, Delete
        </button>
        <button
          @click="hideModal"
          class="bg-gray-300 hover:bg-gray-400 text-gray-800 text-lg px-6 py-3 rounded-full shadow-md font-medium transition-all focus:ring focus:ring-gray-400"
        >
          Cancel
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, defineProps, defineEmits, defineExpose } from "vue";

//  props
const props = defineProps({
  title: {
    type: String,
    default: "",
  },
  message: {
    type: String,
    default: "",
  },
});

//  emits
const emit = defineEmits(["confirm"]);

//  A ref to track modal visibility
const visible = ref(false);

//  methods
function showModal() {
  visible.value = true;
}

function hideModal() {
  visible.value = false;
}

function onConfirm() {
  emit("confirm");
  hideModal();
}

// 4) Expose the methods we want the parent to call
defineExpose({
  showModal,
  hideModal,
});
</script>

Overlay Background: bg-black bg-opacity-70 backdrop-blur-sm: Adds a darker, blurred
background for better focus on the modal. Gradient Modal Container: bg-gradient-to-br
from-red-600 via-red-500 to-red-400: A fiery gradient to emphasize the destructive nature
of deletion. rounded-3xl shadow-2xl: Smoothly rounded corners and deep shadow for a modern
look. hover:scale-105: Interactive scaling effect when hovering over the modal. Close
Button: absolute top-4 right-4: Positioned elegantly in the top-right corner. fa-solid
fa-times text-3xl: Large, prominent close icon. Header Icon: fa-solid fa-trash text-5xl
text-yellow-400: A large trash icon highlighted in yellow. Icon is enclosed in a glowing
circle: bg-white bg-opacity-20 p-4 rounded-full. Typography: Title: text-3xl font-bold
mt-4: Bold and large for emphasis. Message: text-lg text-white text-opacity-90
leading-relaxed: Clean and readable. Action Buttons: Delete Button: bg-red-700
hover:bg-red-600: Bold and vibrant red to emphasize danger. rounded-full shadow-md:
Rounded with a soft shadow for premium feel. focus:ring focus:ring-red-300: Adds focus
accessibility. Cancel Button: bg-gray-300 hover:bg-gray-400: Neutral tone for cancel
action. text-gray-800: Contrasts against the modal's theme. Responsive Design: w-11/12
max-w-lg: Ensures it fits well on all screen sizes. Animation and Interactivity: Scaling
Effect: hover:scale-105 for smooth zoom interaction. Transitions: transition-transform and
duration-500 for smooth animations.
