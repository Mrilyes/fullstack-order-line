<template>
  <!--
    data-table: Container for the entire data table.
    p-4: Applies padding of 1rem on all sides.
    bg-white: Sets the background color to white.
    rounded-lg: Applies large border-radius for rounded corners.
    shadow: Adds a subtle box-shadow for depth.
    relative : position
  -->
  <div class="data-table p-4 bg-white rounded-lg shadow-md relative space-y-6">
    <div class="relative">
      <!-- Loading Overlay -->
      <!-- v-if="loading": Only displays when the loading prop is true.
          absolute inset-0: Positions the overlay to cover the entire table area.
          flex items-center justify-center: Centers the loading spinner both vertically and horizontally.
          bg-white bg-opacity-75: Adds a semi-transparent white background to obscure the table while loading.
          z-10: Ensures the overlay appears above the table. -->
      <div
        v-if="loading || pageLoading"
        class="absolute inset-0 flex items-center justify-center bg-white bg-opacity-80 z-10"
      >
        <svg
          class="animate-spin h-8 w-8 text-blue-500"
          xmlns="http://www.w3.org/2000/svg"
          fill="none"
          viewBox="0 0 24 24"
        >
          <circle
            class="opacity-25"
            cx="12"
            cy="12"
            r="10"
            stroke="currentColor"
            stroke-width="4"
          ></circle>
          <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8H4z"></path>
        </svg>
      </div>
      <!--
      overflow-x-auto: Enables horizontal scrolling if the table exceeds the container's width.
      which Allows horizontal scrolling of the table if its content exceeds the container's width, preventing it from pushing the pagination controls out of view.
    -->

      <!-- Scrollable Table -->
      <div class="overflow-x-auto border border-gray-300 rounded-lg shadow-sm">
        <!--
        min-w-full: Ensures the table takes up the full width of its container.
        bg-white: Sets the table background to white.
        border: Adds a border around the table.
        border-gray-200: Sets the border color to a light gray.
        table-auto: Allows the table to adjust column widths based on content.

      -->
        <table class="min-w-full text-sm md:text-base bg-white border-collapse">
          <thead>
            <tr class="px-4 py-2 bg-gradient-to-r from-blue-900 to-blue-800 text-white">
              <!-- px-4 py-2: Applies horizontal padding of 1rem and vertical padding of 0.5rem.
                bg-gray-200: Sets the header background to a light gray.
                text-left: Aligns text to the left.
                text-xs: Sets the font size to extra small.
                font-semibold: Makes the font weight semi-bold.
                text-gray-700: Sets the text color to a darker gray.
                uppercase: Transforms text to uppercase.
                tracking-wider: Increases letter spacing.
                cursor-pointer: Changes cursor to pointer on hover, indicating interactivity.
                Responsive Text Sizes: Adjusts text size based on screen size (sm:text-sm, md:text-base).

                -->

              <th
                v-for="column in columns"
                :key="column.field"
                class="px-4 py-3 text-left font-semibold uppercase tracking-wide cursor-pointer"
              >
                <div class="flex items-center space-x-2">
                  <!--
                  Displays the column header.
                -->
                  <span>{{ column.header }}</span>
                  <!--
            Search Icon:
                  - ml-2: Applies a left margin of 0.5rem.
                  - flex items-center: Ensures the icon is vertically centered.
                  - space-x-1: Adds horizontal spacing between child elements.
                  - text-white: Sets the icon color to white.
                  - hover:text-blue-300: Changes icon color to light blue on hover.
                  - transition-colors duration-200: Adds a smooth transition for color changes.
                  - cursor-pointer: Indicates interactivity.
                -->
                  <span
                    class="ml-2 flex items-center space-x-1 text-white hover:text-blue-300 transition-colors duration-200 cursor-pointer"
                    @click="toggleSearch(column.field)"
                    :title="`Search ${column.header}`"
                  >
                    <i class="bi bi-search"></i>
                  </span>
                  <!--
                  Sort Icon:
                  - ml-2: Applies a left margin of 0.5rem.
                  - flex items-center: Ensures the icon is vertically centered.
                  - space-x-1: Adds horizontal spacing between child elements.
                  - text-white: Sets the icon color to white.
                  - hover:text-blue-300: Changes icon color to light blue on hover.
                  - transition-colors duration-200: Adds a smooth transition for color changes.
                  - cursor-pointer: Indicates interactivity.
                  - @click.stop: Stops the click event from propagating to parent elements.
                -->

                  <!-- role="button": Indicates that the span behaves like a button.
              aria-label: Provides a descriptive label for screen readers. -->

                  <!-- tabindex="0": Makes the element focusable via keyboard navigation.
                @keydown.enter.prevent: Allows users to toggle sorting by pressing the Enter key while the sort icon is focused. -->
                  <span
                    class="ml-2 flex items-center space-x-1 text-white hover:text-blue-300 transition-colors duration-200 cursor-pointer"
                    @click.stop="toggleSort(column.field)"
                    :title="`Sort ${column.header}`"
                    role="button"
                    aria-label="Sort by {{ column.header }}"
                    tabindex="0"
                    @keydown.enter.prevent="toggleSort(column.field)"
                  >
                    <i :class="getSortIcon(column.field)"></i>
                  </span>
                </div>
                <!--
                Search Input Field:
                mt-2: Adds a top margin of 0.5rem.
                w-full: Sets the input width to 100% of its container.
                px-2 py-1: Applies horizontal padding of 0.5rem and vertical padding of 0.25rem.
                border: Adds a border to the input.
                rounded: Applies a small border-radius for rounded corners.
                focus:outline-none: Removes the default outline on focus.
                focus:ring-2 focus:ring-blue-300: Adds a blue ring around the input on focus for better visibility.
              -->
                <div v-if="searchField === column.field && showSearchRow" class="mt-2">
                  <input
                    v-model="filters[column.field]"
                    class="w-full px-2 py-1 border rounded focus:outline-none focus:ring-2 focus:ring-blue-300 text-black"
                    :placeholder="`Search ${column.header}`"
                  />
                </div>
              </th>
            </tr>
          </thead>
          <tbody>
            <!-- bg-blue-100: Highlights the row with a light blue background if selected.
              hover:bg-gray-50: Changes the row background to an even lighter gray on hover.
              cursor-pointer: Changes cursor to pointer on hover, indicating interactivity. -->
            <tr v-if="paginatedData.length === 0">
              <td :colspan="columns.length" class="text-center py-4 text-gray-500">
                No records found.
              </td>
            </tr>
            <tr
              v-for="row in paginatedData"
              :key="row.id"
              @click.stop="selectRow(row)"
              :class="{
                'bg-blue-300': localSelectedRow && localSelectedRow.id === row.id,
                'hover:bg-blue-100': true,
                'cursor-pointer': true,
              }"
              class="transition-colors duration-200"
            >
              <!-- px-4 py-2: Applies horizontal padding of 1rem and vertical padding of 0.5rem.
                border-b: Adds a bottom border to the cell.
                text-sm: Sets the font size to small.
                text-gray-700: Sets the text color to a darker gray. -->
              <td
                v-for="column in columns"
                :key="column.field"
                class="px-4 py-2 border-b text-sm text-gray-700"
              >
                <slot :name="`cell-${column.field}`" :row="row">
                  {{ row[column.field] }}
                </slot>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!--
      Pagination Controls:
      flex: Applies display: flex.
      justify-center: Centers the pagination controls horizontally.
      mt-4: Adds a top margin of 1rem.
      space-x-2: Adds horizontal spacing of 0.5rem between pagination buttons.
      flex-wrap: Allows the pagination buttons to wrap to the next line on smaller screens, preventing overflow.
      space-x-2 space-y-2 sm:space-y-0: Adds vertical spacing (space-y-2) between wrapped lines on smaller screens and removes it on sm (small) screens and above,
      maintaining horizontal spacing (space-x-2).
    -->
    <div class="flex flex-wrap justify-center items-center gap-2 mt-4">
      <!--
        Previous Button:
        px-3 py-1: Applies horizontal padding of 0.75rem and vertical padding of 0.25rem.
        bg-blue-500: Sets the background color to blue.
        text-white: Sets the text color to white.
        rounded: Applies a small border-radius for rounded corners.
        disabled:bg-blue-300: Changes the background color to a lighter blue when disabled.
        hover:bg-blue-600: Darkens the blue on hover.
        transition-colors duration-200: Adds a smooth transition for color changes.
        rounded-full: Makes the buttons fully rounded, giving them a pill-shaped appearance.
      -->
      <button
        class="px-4 py-2 bg-gradient-to-r from-blue-900 to-blue-800 text-white rounded-full shadow-md hover:from-blue-700 hover:to-blue-400 disabled:from-gray-300 disabled:to-gray-300 disabled:cursor-not-allowed transition-all"
        :disabled="currentPage === 1"
        @click="goToPage(1)"
        aria-label="First Page"
      >
        First
      </button>

      <button
        class="px-4 py-2 bg-gradient-to-r from-blue-900 to-blue-800 text-white rounded-full shadow-md hover:from-blue-700 hover:to-blue-400 disabled:from-gray-300 disabled:to-gray-300 disabled:cursor-not-allowed transition-all"
        :disabled="currentPage === 1"
        @click="goToPage(currentPage - 1)"
        aria-label="Previous Page"
      >
        Previous
      </button>

      <!--
        Page Number Buttons:
        px-3 py-1: Applies horizontal padding of 0.75rem and vertical padding of 0.25rem.
        bg-gray-200: Sets the background color to light gray.
        text-gray-700: Sets the text color to dark gray.
        rounded: Applies a small border-radius for rounded corners.
        hover:bg-gray-300: Lightens the gray on hover.
        transition-colors duration-200: Adds a smooth transition for color changes.
        Conditional Classes:
        'bg-blue-500 text-white': Highlights the current page with a blue background and white text.
        flex-shrink-0: Prevents buttons from shrinking below their defined size, maintaining readability and touch targets.
        shadow-md: Adds a medium shadow for depth.

      -->
      <template v-for="page in visiblePages">
        <!-- Page Numbers -->
        <button
          :key="page !== '...' ? page + '-pagination' : 'ellipsis-' + page"
          v-if="page !== '...'"
          class="px-4 py-2 rounded-full shadow-md text-sm md:text-base transition-all"
          :class="{
            'bg-gradient-to-r from-blue-900 to-blue-800  hover:from-blue-700 hover:to-blue-400 disabled:from-gray-300 disabled:to-gray-300 text-white':
              currentPage === page,
            'bg-gray-100 text-gray-700 hover:bg-gray-200': currentPage !== page,
          }"
          @click="goToPage(page)"
          :aria-label="`Page ${page}`"
          :aria-current="currentPage === page ? 'page' : undefined"
          tabindex="0"
          @keydown.enter.prevent="goToPage(page)"
        >
          {{ page }}
        </button>

        <!-- Ellipsis -->
        <button
          v-else
          :key="'ellipsis-' + page"
          class="px-4 py-2 rounded-full bg-gray-300 text-gray-700 shadow-md cursor-not-allowed"
          disabled
        >
          ...
        </button>
      </template>

      <!--
        Next Button:
        px-3 py-1: Applies horizontal padding of 0.75rem and vertical padding of 0.25rem.
        bg-blue-500: Sets the background color to blue.
        text-white: Sets the text color to white.
        rounded: Applies a small border-radius for rounded corners.
        disabled:bg-blue-300: Changes the background color to a lighter blue when disabled.
        hover:bg-blue-600: Darkens the blue on hover.
        transition-colors duration-200: Adds a smooth transition for color changes.
      -->
      <button
        class="px-4 py-2 bg-gradient-to-r from-blue-900 to-blue-800 text-white rounded-full shadow-md hover:from-blue-700 hover:to-blue-400 disabled:from-gray-300 disabled:to-gray-300 disabled:cursor-not-allowed transition-all"
        :disabled="currentPage === totalPages || totalPages === 0"
        @click="goToPage(currentPage + 1)"
        aria-label="Next Page"
      >
        Next
      </button>

      <button
        class="px-4 py-2 bg-gradient-to-r from-blue-900 to-blue-800 text-white rounded-full shadow-md hover:from-blue-700 hover:to-blue-400 disabled:from-gray-300 disabled:to-gray-300 disabled:cursor-not-allowed transition-all"
        :disabled="currentPage === totalPages || totalPages === 0"
        @click="goToPage(totalPages)"
        aria-label="Last Page"
      >
        Last
      </button>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, watch, computed } from "vue";
import { debounce } from "lodash";

// Props
const props = defineProps({
  data: {
    type: Array,
    default: () => [], // Default to an empty array
    required: true,
  },
  columns: {
    type: Array,
    required: true,
  },
  selectedRow: {
    type: Object, // Explicitly declare the type
    default: null, // Default value if not provided
  },
  loading: {
    type: Boolean,
    default: false,
  },
});

// Emits
const emit = defineEmits(["row-selected"]);

// Reactive state for filters, toggles, and pagination
const filters = reactive({});
const showSearchRow = ref(false); // Controls visibility of the search row
const searchField = ref(null); // Tracks the currently active search field
const sortField = ref(null); // Tracks the field used for sorting
const sortOrder = ref(null); // Tracks the order of sorting (asc/desc)
const currentPage = ref(1); // Current page for pagination
const rowsPerPage = ref(5); // Number of rows per page
const localSelectedRow = ref(null); // Local tracking for selected row
const pageLoading = ref(false); // Loading state for pagination changes

// Sync localSelectedRow with the parent's selectedRow prop
watch(
  () => props.selectedRow,
  (newValue) => {
    localSelectedRow.value = newValue;
  },
  { immediate: true }
);

// Debugging: Log changes to currentPage
watch(currentPage, (newPage) => {
  console.log(`Current page updated to: ${newPage}`);
});

// Debugging: Log changes to sortField
watch(sortField, (newField) => {
  console.log(`Sort field changed to: ${newField}`);
});

// Debugging: Log changes to sortOrder
watch(sortOrder, (newOrder) => {
  console.log(`Sort order changed to: ${newOrder}`);
});

// Watch for changes to filters and debounce the update logic
// Debounce Function: Delays the execution of the filter logic until the user has stopped typing for a specified duration (300ms in this case), reducing the number of times the filter is applied.
// Reset currentPage: Optionally resets to the first page when filters change to ensure relevant results are shown.
watch(
  filters,
  debounce(() => {
    currentPage.value = 1; // Reset to the first page when filters change
  }, 300), // 300ms debounce delay
  { deep: true }
);

// Computed property for filtering data based on filters
const filteredData = computed(() => {
  if (!Array.isArray(props.data)) {
    console.error("Data passed to DataTable is not an array:", props.data);
    return [];
  }
  return props.data.filter((item) => {
    return props.columns.every((col) => {
      const fieldValue = String(item[col.field] || "").toLowerCase();
      const filterValue = String(filters[col.field] || "").toLowerCase();
      return fieldValue.includes(filterValue);
    });
  });
});

// Computed property for sorting filtered data
const filteredAndSortedData = computed(() => {
  const sortedData = [...filteredData.value];
  if (sortField.value) {
    sortedData.sort((a, b) => {
      if (a[sortField.value] > b[sortField.value]) {
        return sortOrder.value === "asc" ? 1 : -1;
      }
      if (a[sortField.value] < b[sortField.value]) {
        return sortOrder.value === "asc" ? -1 : 1;
      }
      return 0;
    });
  }
  return sortedData;
});

// Computed property for paginating data
const paginatedData = computed(() => {
  const start = (currentPage.value - 1) * rowsPerPage.value;
  const end = start + rowsPerPage.value;
  return filteredAndSortedData.value.slice(start, end);
});

// Total number of pages based on filtered data
const totalPages = computed(() => {
  return Math.ceil(filteredData.value.length / rowsPerPage.value);
});

// Computed property for visible page numbers with ellipses

// visiblePages Computation: Limits the number of visible page buttons,
// showing only a subset around the current page with ellipses (...) to indicate hidden pages.
// delta: Determines how many page numbers to display on either side of the currentPage.
// range: Collects all the page numbers that should be displayed.
// rangeWithDots: Inserts ellipses (...) where pages are skipped.
const visiblePages = computed(() => {
  const total = totalPages.value;
  const current = currentPage.value;
  const delta = 2; // Number of pages to show around the current page
  const range = [];
  const rangeWithDots = [];
  let l;

  for (let i = 1; i <= total; i++) {
    if (i === 1 || i === total || (i >= current - delta && i <= current + delta)) {
      range.push(i);
    }
  }

  range.forEach((i) => {
    if (l) {
      if (i - l === 2) {
        rangeWithDots.push(l + 1);
      } else if (i - l !== 1) {
        rangeWithDots.push("...");
      }
    }
    rangeWithDots.push(i);
    l = i;
  });

  return rangeWithDots;
});

// Toggle the visibility of the search row
const toggleSearch = (field) => {
  if (searchField.value === field && showSearchRow.value) {
    searchField.value = null;
    showSearchRow.value = false;
  } else {
    searchField.value = field;
    showSearchRow.value = true;
  }
};

// Toggle sorting logic for a given field
const toggleSort = (field) => {
  if (sortField.value === field) {
    sortOrder.value = sortOrder.value === "asc" ? "desc" : "asc";
  } else {
    sortField.value = field;
    sortOrder.value = "asc";
  }
};

// Get the appropriate sort icon for a field
const getSortIcon = (field) => {
  if (sortField.value === field) {
    return sortOrder.value === "asc"
      ? "bi bi-sort-alpha-up text-white"
      : "bi bi-sort-alpha-down-alt text-white";
  }
  return "bi bi-sort-alpha-up text-white"; // Default sort icon
};

// Handle pagination navigation
const goToPage = (page) => {
  if (page === "...") return; // Do nothing on ellipsis click
  if (page >= 1 && page <= totalPages.value) {
    pageLoading.value = true; // Start loading
    currentPage.value = page;
    // Simulate a delay for loading spinner (e.g., 300ms)
    setTimeout(() => {
      pageLoading.value = false; // Stop loading after delay
    }, 300);
  }
};
// Select a row and emit the selected row to the parent
// Parent communication: When a row is clicked, its data is passed to the parent via row-selected.
const selectRow = (row) => {
  localSelectedRow.value = row;
  emit("row-selected", row);
};
</script>
