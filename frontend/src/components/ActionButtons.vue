<template>
  <!-- Button Size:

    Reduced padding: Changed px-3 py-1 to px-2 py-1.

    Reduced text size: Changed text-lg md:text-base to text-sm.

    Icon Centering:

    Added justify-center and items-center to ensure the icon and text are centered inside the button.

    Space Between Buttons:

    Reduced the gap between buttons: Changed gap-2 to gap-1 in the parent div.

    Icon Size:

    Adjusted icon size: Changed text-lg md:text-base to text-base for the icon. -->

  <!-- Button Padding and Text Size:

    Added responsive padding and text size adjustments:

    Small screens (sm:):

    Padding: px-2 py-1

    Text size: text-xs

    Medium screens (md:):

    Padding: px-3 py-1.5

    Text size: text-sm

    Large screens (lg:):

    Padding: px-4 py-2

    Text size: text-base

    con Size:

    Added responsive icon size adjustments:

    Small screens (sm:):

    Icon size: text-sm

    Medium screens (md:):

    Icon size: text-base

    Large screens (lg:):

    Icon size: text-lg
    -->

  <div class="flex flex-wrap gap-0.1">
    <button
      v-for="button in props.buttons"
      :key="button.action"
      :disabled="button.disabled"
      @click="emit('button-clicked', button.action)"
      :class="[
        'flex items-center justify-center bg-[#003f7f] text-white border-[#003f7f] gap-0.1 px-3 py-0.6 rounded-md font-medium text-sm transition-colors m-1.5 mb-1 md:px-3 md:py-2 md:text-xs', // Responsive adjustments for smaller screens
        'md:px-3 md:py-1.5 md:text-sm', // Responsive adjustments for medium screens
        'lg:px-4 lg:py-2 lg:text-base', // Responsive adjustments for larger screens
        button.disabled
          ? 'bg-gray-400 cursor-not-allowed opacity-50'
          : button.customStyle || '', // Default style if customStyle is not provided
      ]"
      :title="button.placeholder"
    >
      <span
        v-if="button.icon"
        :class="[button.icon, 'text-base sm:text-sm md:text-base lg:text-lg']"
      ></span>
      <span>{{ button.label }}</span>
    </button>
  </div>
</template>

<script setup>
const props = defineProps({
  buttons: {
    type: Array,
    required: true,
  },
  isCreateMode: {
    type: Boolean,
    default: false,
  },
});

const emit = defineEmits(["button-clicked"]);
</script>
