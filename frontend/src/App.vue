<template>
  <!-- w-full: Sets the width of the <main> element to 100% of its parent, ensuring it spans the entire viewport.
    Removed container mx-auto: Eliminates the max-width constraint and horizontal centering, allowing the content to utilize the full width. -->
  <div class="flex flex-col min-h-screen">
    <main class="flex-1 w-full">
      <router-view />
    </main>
  </div>
</template>
