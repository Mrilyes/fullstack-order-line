<template>
  <!-- flex: Applies display: flex;.
justify-between: Distributes space between items.
items-center: Vertically centers items.
py-2 px-4: Applies vertical padding (0.5rem) and horizontal padding (1rem).
bg-[#003f7f]: Sets the background color to #003f7f. (Ensure your Tailwind version supports arbitrary values. If not, consider adding this color to your tailwind.config.js.)
text-white: Sets the text color to white.
h-16: Sets the height to 4rem (64px), which is close to 65px.
shadow-md: Applies a medium box-shadow. -->
  <!-- py-2 px-4 sm:py-4 sm:px-6: Increases vertical and horizontal padding on sm and larger screens. -->
  <nav
    class="flex justify-between items-center py-2 px-4 sm:py-4 sm:px-6 bg-navbar-bg text-white h-14 shadow-md"
  >
    <!-- Left Section: Burger Button and Logo -->

    <!-- flex: Applies display: flex;.
    items-center: Vertically centers items. -->
    <div class="flex items-center">
      <!-- Burger Button -->

      <!-- bg-transparent: Removes background.
          border-none: Removes border.
          text-white: Sets text color to white.
          text-2xl: Sets font size to 1.5rem.
          cursor-pointer: Changes cursor to pointer on hover.
          px-3: Applies horizontal padding (0.75rem). (Adjusted from 10px to the closest Tailwind spacing.)
          hover:text-blue-300: Changes text color on hover to a light blue shade.
          focus:outline-none: Removes default outline on focus for a cleaner look. -->
      <button
        class="bg-transparent border-none text-white text-3xl cursor-pointer px-0.5 hover:text-blue-300 focus:outline-none"
        id="burgerButton"
        @click="toggleBurgerButton"
        aria-label="Toggle Sidebar"
      >
        <i class="bi bi-list"></i>
      </button>

      <!-- Logo -->

      <!--h-10 w-auto sm:h-12: Sets the logo height to 2.5rem (40px)
      on small screens and 3rem (48px) on sm and larger, maintaining aspect ratio with w-auto. -->
      <router-link to="/dashboard">
        <img
          src="@/assets/GroupeSopal.png"
          alt="Navbar Logo"
          class="ml-2 h-10 w-auto sm:h-12"
        />
      </router-link>

      <!-- Fullscreen Toggle -->

      <!-- bg-transparent: Removes background.
        border-none: Removes border.
        text-white: Sets text color to white.
        text-xl: Sets font size to 1.25rem (1.2rem approximation).
        pl-5: Applies left padding of 1.25rem (20px).
        cursor-pointer: Changes cursor to pointer on hover.
        hover:text-blue-300: Changes text color on hover to a light blue shade.
        focus:outline-none: Removes default outline on focus. -->

      <button
        class="bg-transparent border-none text-white text-xl pl-2 cursor-pointer hover:text-blue-300 focus:outline-none"
        id="fullscreenButton"
        @click="toggleFullscreen"
        :title="isFullscreen ? 'Exit Fullscreen' : 'Enter Fullscreen'"
        aria-label="Toggle Fullscreen"
      >
        <i :class="isFullscreen ? 'bi bi-fullscreen-exit' : 'bi bi-fullscreen'"></i>
      </button>
    </div>

    <!-- Right Section: Home Icon and Username -->

    <!-- flex: Applies display: flex;.
    items-center: Vertically centers items.
    space-x-4: Adds horizontal spacing (1rem) between child elements.
    For the <p> tag:

    text-white: Sets text color to white.
    text-base: Sets font size to 1rem.
    For the Home button (router-link):

    bg-transparent: Removes background.
    border-none: Removes border.
    text-white: Sets text color to white.
    text-base: Sets font size to 1rem.
    cursor-pointer: Changes cursor to pointer on hover.
    px-3: Applies horizontal padding (0.75rem).
    hover:text-blue-300: Changes text color on hover.
    focus:outline-none: Removes default outline on focus. -->
    <div class="flex items-center space-x-4">
      <!-- hidden sm:block: Hides the element on screens smaller than sm (640px) and displays it as a block on sm and larger. -->
      <p id="username" class="text-white text-base hidden sm:block -mb-0.5 items-center">
        connected as : GHALILA ILYES
      </p>
      <!-- text-xl : xlarge
      font-size: 1.25rem; /* 20px */
      line-height: 1.75rem; /* 28px */ -->
      <router-link
        to="/homePage"
        class="bg-transparent border-none text-white text-xl cursor-pointer px-1 hover:text-blue-300 focus:ring-2 focus:ring-blue-300 flex"
        id="homeButton"
        aria-label="Go to Home"
        title="Home"
      >
        <i class="bi bi-house-door-fill text-current"></i>
      </router-link>
    </div>
  </nav>
</template>

<script setup>
import { ref } from "vue";

const isFullscreen = ref(false);

const toggleFullscreen = () => {
  if (!isFullscreen.value) {
    document.documentElement.requestFullscreen?.() ||
      document.documentElement.webkitRequestFullscreen?.();
  } else {
    document.exitFullscreen?.() || document.webkitExitFullscreen?.();
  }
  isFullscreen.value = !isFullscreen.value;
};

const toggleBurgerButton = () => {
  console.log("burger button launched");
};
</script>
