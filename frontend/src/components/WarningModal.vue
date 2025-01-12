<template>
  <div
    v-if="props.visible"
    class="fixed inset-0 bg-black bg-opacity-70 backdrop-blur-sm flex items-center justify-center z-50 transition-opacity duration-300"
  >
    <div
      class="relative bg-gradient-to-br from-blue-700 via-blue-600 to-blue-500 text-white p-8 md:p-12 rounded-3xl shadow-2xl transform scale-95 hover:scale-105 transition-transform duration-500 max-w-lg w-full"
    >
      <!-- Close Button -->
      <button
        @click="onLeave"
        class="absolute top-4 right-4 text-white text-opacity-80 hover:text-opacity-100 transition-all"
      >
        <i class="fa-solid fa-times text-2xl"></i>
      </button>

      <!-- Modal Header -->
      <div class="text-center mb-8">
        <div
          class="bg-white bg-opacity-20 p-4 rounded-full inline-flex items-center justify-center shadow-lg"
        >
          <i class="fa-solid fa-exclamation-triangle text-yellow-400 text-4xl"></i>
        </div>
        <h3 class="text-3xl font-bold mt-4">Unsaved Changes</h3>
      </div>

      <!-- Modal Body -->
      <p class="text-center text-lg text-white text-opacity-90 leading-relaxed">
        Your unsaved work will be lost!
      </p>

      <!-- Modal Actions -->
      <div class="mt-8 flex flex-col sm:flex-row justify-center sm:justify-between gap-4">
        <!-- Stay Button -->
        <button
          @click="onStay"
          class="bg-green-500 hover:bg-green-400 text-white text-lg px-6 py-3 rounded-full font-medium shadow-md transition-all focus:outline-none focus:ring focus:ring-green-300"
        >
          Stay
        </button>

        <!-- Leave Button -->
        <button
          @click="onLeave"
          class="bg-red-600 hover:bg-red-500 text-white text-lg px-6 py-3 rounded-full font-medium shadow-md transition-all focus:outline-none focus:ring focus:ring-red-300"
        >
          Leave
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
const props = defineProps({
  visible: {
    type: Boolean,
    default: false,
  },
});
const emit = defineEmits(["stay", "leave"]);

const onStay = () => {
  console.log("User chose to stay on the page.");
  emit("stay"); // Emit stay event
};
const onLeave = () => {
  console.log("User chose to leave the page.");
  emit("leave"); // Emit leave event
};
</script>

// Overlay Background: // bg-black bg-opacity-60 backdrop-blur-md: A darker,
semi-transparent overlay with a subtle blur effect to enhance focus on the modal. //
transition-opacity duration-300: Smooth fade-in animation for the overlay. // Modal
Container: // bg-gradient-to-br from-blue-700 via-blue-600 to-blue-500: A sophisticated
gradient for the modal's background. // rounded-2xl shadow-2xl: Enhanced rounded corners
and deeper shadow for a modern card-like appearance. // hover:scale-100: Interactive
scaling effect for better engagement. // Close Button: // Positioned at the top-right
corner for easy dismissal. // fa-solid fa-times text-2xl: A prominent close icon with
hover interactivity. // Header Icon: // bg-white bg-opacity-20: A translucent circle
enclosing the warning icon. // text-yellow-400 text-4xl: Larger icon size for prominence.
// Typography: // Title: text-3xl font-bold: Clean and bold for emphasis. // Body Text:
text-lg text-opacity-90 leading-relaxed: Improved readability with slightly muted white
text. // Action Buttons: // Stay Button: // bg-green-500 hover:bg-green-400: Vibrant green
for positive action. // focus:ring focus:ring-green-300: Accessible focus state for
keyboard navigation. // Leave Button: // bg-red-600 hover:bg-red-500: Bold red for
destructive action. // focus:ring focus:ring-red-300: Consistent with the green button. //
Responsive Layout: // max-w-xl w-full: Ensures the modal scales well on all screen sizes.
// flex flex-col sm:flex-row: Stacks buttons vertically on small screens and aligns them
horizontally on larger screens. // Interactivity and Accessibility: // Buttons have focus
rings for keyboard accessibility. // Hover and transition effects provide visual feedback
for interactions.
