<template>
  <div
    v-if="visible"
    class="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-70 backdrop-blur-sm transition-opacity duration-300"
    @click.self="closeModal"
  >
    <!-- Modal Content -->
    <div
      class="relative bg-gradient-to-br from-gray-900 via-gray-800 to-gray-700 text-white rounded-3xl shadow-2xl w-full max-w-4xl max-h-[85%] overflow-y-auto p-8 transform transition-transform duration-500 scale-95 hover:scale-100"
    >
      <!-- Header -->
      <!-- <div class="flex justify-between items-center mb-6 border-b border-gray-600 pb-4"> -->
      <!-- <h3 class="text-3xl font-semibold">{{ props.title || "Product Selection" }}</h3> -->
      <button
        @click="closeModal"
        class="absolute top-4 right-4 mb-3 text-gray-300 hover:text-gray-400 transition-all"
      >
        <i class="fa-solid fa-times text-2xl"></i>
      </button>
      <!-- </div> -->

      <!-- Table -->
      <table class="w-full text-sm md:text-base border-separate border-spacing-y-3">
        <thead>
          <tr class="bg-gradient-to-r from-blue-900 to-blue-800 text-white">
            <th
              @click="sortTable('articleId')"
              class="p-4 text-center cursor-pointer font-semibold uppercase tracking-wide"
            >
              ID
              <i :class="getSortIcon('articleId')" class="ml-2"></i>
            </th>
            <th
              @click="sortTable('name')"
              class="p-4 text-center cursor-pointer font-semibold uppercase tracking-wide"
            >
              Product Name
              <i :class="getSortIcon('name')" class="ml-2"></i>
            </th>
          </tr>
          <tr>
            <th class="p-4">
              <div class="relative">
                <i
                  class="fa-solid fa-search absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400"
                ></i>
                <input
                  v-model="filters.articleId"
                  placeholder="Search ID"
                  @input="applyFilters"
                  class="w-full bg-gray-800 text-white placeholder-gray-400 border border-gray-600 rounded-full py-2 pl-10 focus:ring-2 focus:ring-blue-500 focus:outline-none"
                />
              </div>
            </th>
            <th class="p-4">
              <div class="relative">
                <i
                  class="fa-solid fa-search absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400"
                ></i>
                <input
                  v-model="filters.name"
                  placeholder="Search Product Name"
                  @input="applyFilters"
                  class="w-full bg-gray-800 text-white placeholder-gray-400 border border-gray-600 rounded-full py-2 pl-10 focus:ring-2 focus:ring-blue-500 focus:outline-none"
                />
              </div>
            </th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="product in paginatedProducts"
            :key="product.id"
            @dblclick="selectProduct(product)"
            class="bg-gray-800 hover:bg-gray-700 transition-all border border-gray-700 rounded-lg cursor-pointer"
          >
            <td class="p-4 text-center">{{ product.articleId }}</td>
            <td class="p-4 text-center">{{ product.name }}</td>
          </tr>
        </tbody>
      </table>

      <!-- No Results Message -->
      <div v-if="filteredProducts.length === 0" class="text-center text-gray-400 py-6">
        No products found.
      </div>

      <!-- Pagination Controls -->
      <div class="flex justify-between items-center mt-6">
        <button
          @click="prevPage"
          :disabled="currentPage === 1"
          class="px-4 py-2 bg-gradient-to-r from-blue-700 to-blue-600 text-white rounded-full hover:from-blue-600 hover:to-blue-500 disabled:from-gray-600 disabled:to-gray-500 disabled:cursor-not-allowed transition-all"
        >
          Previous
        </button>
        <span class="text-sm text-gray-400">
          Page {{ currentPage }} of {{ totalPages }}
        </span>
        <button
          @click="nextPage"
          :disabled="currentPage === totalPages || totalPages === 0"
          class="px-4 py-2 bg-gradient-to-r from-blue-700 to-blue-600 text-white rounded-full hover:from-blue-600 hover:to-blue-500 disabled:from-gray-600 disabled:to-gray-500 disabled:cursor-not-allowed transition-all"
        >
          Next
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch } from "vue";

const props = defineProps({
  title: {
    type: String,
    default: "Product Name",
  },
  visible: {
    type: Boolean,
    default: false,
  },
  products: {
    type: Array,
    default: () => [],
  },
});

const emit = defineEmits(["close", "product-selected"]);

// Reactive state
const filters = ref({
  articleId: "",
  name: "",
});

const sortKey = ref("");
const sortOrder = ref("asc"); // 'asc' or 'desc'
const filteredProducts = ref([...props.products]); // Holds the filtered/sorted products

// Pagination state
const currentPage = ref(1);
const itemsPerPage = ref(4); // Number of items per page

// Computed properties
const totalPages = computed(() =>
  Math.ceil(filteredProducts.value.length / itemsPerPage.value)
);

const paginatedProducts = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value;
  const end = start + itemsPerPage.value;
  return filteredProducts.value.slice(start, end);
});

// Watchers
watch(
  () => props.products,
  (newProducts) => {
    filteredProducts.value = [...newProducts];
    applyFilters();
  },
  { immediate: true }
);

// Methods
function applyFilters() {
  const { articleId, name } = filters.value;

  filteredProducts.value = props.products.filter((product) => {
    return (
      (!articleId || product.articleId.toString().includes(articleId.toString())) &&
      (!name || product.name.toLowerCase().includes(name.toLowerCase()))
    );
  });

  // Reset to the first page after filtering
  currentPage.value = 1;

  // Reapply sorting after filtering
  if (sortKey.value) {
    sortTable(sortKey.value);
  }
}

function sortTable(key) {
  if (!key) return;

  sortOrder.value = sortKey.value === key && sortOrder.value === "asc" ? "desc" : "asc";
  sortKey.value = key;

  filteredProducts.value.sort((a, b) => {
    const result =
      a[key].toString().localeCompare(b[key].toString(), undefined, {
        numeric: true,
      }) * (sortOrder.value === "asc" ? 1 : -1);
    return result;
  });
}

function getSortIcon(key) {
  if (sortKey.value !== key) return "";
  return sortOrder.value === "asc"
    ? "fa-solid fa-arrow-up animate-bounce"
    : "fa-solid fa-arrow-down animate-bounce";
}

function selectProduct(product) {
  emit("product-selected", product);
}

function closeModal() {
  // Reset filters
  filters.value = {
    articleId: "",
    name: "",
  };

  // Reset sort state
  sortKey.value = "";
  sortOrder.value = "asc";

  // Reset filteredProducts to the original products
  filteredProducts.value = [...props.products];

  // Reset pagination
  currentPage.value = 1;

  // Emit the close event
  emit("close");
}

// Pagination methods
function prevPage() {
  if (currentPage.value > 1) {
    currentPage.value--;
  }
}

function nextPage() {
  if (currentPage.value < totalPages.value) {
    currentPage.value++;
  }
}
</script>
