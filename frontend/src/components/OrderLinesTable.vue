<template>
  <div class="order-lines">
    <!-- Add Row Button -->
    <button
      @click="addRow"
      class="bg-gradient-to-r mb-2 mt-6 bg-blue-900 text-white px-4 py-2 rounded-full hover:from-blue-700 hover:to-blue-400 transition-colors"
    >
      <i class="fa-solid fa-plus"></i>
    </button>

    <!-- Scrollable Container for Table -->
    <div class="overflow-y-auto max-h-96 border border-gray-200 rounded-lg shadow-md">
      <table class="w-full table-auto text-sm md:text-base bg-white border-collapse">
        <thead>
          <tr class="bg-gradient-to-r from-blue-900 to-blue-800 text-white">
            <th class="p-3 text-center w-1/4 font-semibold uppercase tracking-wide">
              Name
            </th>
            <th class="p-3 text-center w-1/6 font-semibold uppercase tracking-wide">
              Quantity
            </th>
            <th class="p-3 text-center w-1/6 font-semibold uppercase tracking-wide">
              Price
            </th>
            <th class="p-3 text-center w-1/6 font-semibold uppercase tracking-wide">
              Actions
            </th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="(line, index) in paginatedOrderLines"
            :key="line.orderLineId || index"
            class="border-t border-gray-300 hover:bg-gray-100 transition-colors"
          >
            <!-- Name Column -->
            <td class="p-3">
              <div class="relative">
                <input
                  v-model="line.productName"
                  placeholder="Product Name"
                  readonly
                  @click="openModal(index)"
                  class="w-full p-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring focus:ring-blue-300"
                />
                <button
                  class="fa-solid fa-search absolute right-3 top-1/2 transform -translate-y-1/2 text-blue-900 cursor-pointer"
                ></button>
              </div>
            </td>
            <!-- Quantity Column -->
            <td class="p-3">
              <input
                v-model.number="line.quantity"
                type="number"
                placeholder="Quantity"
                @input="updateLocalOrderLine(line, index)"
                class="w-full p-2 border border-gray-300 rounded-md shadow-sm text-center focus:outline-none focus:ring focus:ring-blue-300"
              />
            </td>
            <!-- Price Column -->
            <td class="p-3">
              <input
                v-model.number="line.price"
                type="number"
                placeholder="Price"
                @input="updateLocalOrderLine(line, index)"
                class="w-full p-2 border border-gray-300 rounded-md shadow-sm text-center focus:outline-none focus:ring focus:ring-blue-300"
              />
            </td>
            <!-- Actions Column -->
            <td class="p-3 flex justify-center">
              <button
                @click="deleteOrderLine(line.orderLineId, index)"
                class="bg-red-600 text-white px-4 py-2 rounded-full shadow-md hover:bg-red-500 transition-all"
              >
                <i class="fa-solid fa-trash"></i>
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- No Results Message -->
    <div v-if="totalPages === 0" class="text-center text-gray-400 py-6">
      No Order Line found.
    </div>

    <!-- Pagination Controls -->
    <div class="flex justify-between items-center mt-4">
      <!-- Previous Button -->
      <button
        @click="prevPage"
        :disabled="currentPage === 1"
        class="px-4 py-2 bg-gradient-to-r from-blue-900 to-blue-800 text-white rounded-full shadow-md hover:from-blue-600 hover:to-blue-400 disabled:from-gray-400 disabled:to-gray-400 disabled:cursor-not-allowed transition-all"
      >
        Previous
      </button>

      <!-- Current Page Display -->
      <span class="text-lg text-gray-700">
        Page {{ currentPage }} of {{ totalPages }}
      </span>

      <!-- Next Button -->
      <button
        @click="nextPage"
        :disabled="currentPage === totalPages || totalPages === 0"
        class="px-4 py-2 bg-gradient-to-r from-blue-900 to-blue-800 text-white rounded-full shadow-md hover:from-blue-600 hover:to-blue-400 disabled:from-gray-400 disabled:to-gray-400 disabled:cursor-not-allowed transition-all"
      >
        Next
      </button>
    </div>

    <!-- Article Selection Modal -->
    <ArticleSelectionModal
      :visible="showModal"
      :products="articles"
      @product-selected="handleArticleSelect"
      @close="closeModal"
    />
    <WarningModal :visible="showWarningModal" @stay="handleStay" @leave="handleLeave" />
  </div>
</template>

<script setup>
import ArticleSelectionModal from "@/components/ArticleSelectionModal.vue";
import WarningModal from "@/components/WarningModal.vue";
import apiServices from "@/services/apiServices";
import { useToast } from "vue-toastification";

import { ref, reactive, watch, onMounted, onBeforeUnmount, computed } from "vue";

const props = defineProps({
  orderLines: {
    type: Array,
    default: () => [],
  },
});

// Toast
const toast = useToast();

const emit = defineEmits([
  "unsaved-changes",
  "update-order-lines",
  "deleted-order-lines",
]);

// states

// reactive is ideal for objects or arrays because it makes the entire object or array reactive.
// This means that any nested properties or elements within the object or array will also be reactive.

// localOrderLines: This is an array of order lines, where each order line is an object.

// originalOrderLines: This is also an array used to track the original state of the order lines for comparison.
// Using reactive ensures that the entire array and its nested objects are reactive.

// ref is ideal for primitive values (like boolean, number, string) or when you need a reactive reference to a single value
// You can use ref for arrays or objects, but it requires accessing the value via .value, which can be cumbersome for deeply nested structures

// reactive is designed for objects and arrays, not primitives.
// If you try to use reactive with a primitive, it won't work as expected because Vue's reactivity system relies on object properties

const showModal = ref(false);
const selectedRowIndex = ref(null);
const localOrderLines = reactive([...props.orderLines]); // Local copy of orderLines
const originalOrderLines = reactive(JSON.parse(JSON.stringify(props.orderLines))); // Original copy
const articles = ref([]);
const showWarningModal = ref(false);
const hasUnsavedChanges = ref(false); // Track whether there are unsaved changes
const deletedOrderLines = reactive([]); // Track rows marked for deletion

// Pagination state
const currentPage = ref(1);
const itemsPerPage = ref(4); // Number of items per page

// Computed properties for pagination
const totalPages = computed(() => Math.ceil(localOrderLines.length / itemsPerPage.value));
const paginatedOrderLines = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value;
  const end = start + itemsPerPage.value;
  return localOrderLines.slice(start, end);
});

watch(
  () => props.orderLines,
  (newValue) => {
    localOrderLines.splice(0, localOrderLines.length, ...newValue);
    originalOrderLines.value = JSON.parse(JSON.stringify(newValue));
  },
  { immediate: true }
);

watch(
  localOrderLines,
  () => {
    if (hasChanges()) {
      hasUnsavedChanges.value = true;
      emit("unsaved-changes", true);
    } else {
      hasUnsavedChanges.value = false;
      emit("unsaved-changes", false);
    }
  },
  { deep: true }
);

const hasChanges = () => {
  return JSON.stringify(localOrderLines) !== JSON.stringify(originalOrderLines);
};

const fetchArticles = async () => {
  try {
    const response = await apiServices.get("https://localhost:7128/api/Article");
    articles.value = response;
  } catch (error) {
    console.error("Error fetching Orderline table:", error);
  }
};
// Mark the table as changed so the parent knows
const detectUnsavedChanges = () => {
  // console.log("Unsaved changes detected in order lines table.");
  if (hasChanges()) {
    hasUnsavedChanges.value = true;
    emit("unsaved-changes", true);
  } else {
    hasUnsavedChanges.value = false;
    emit("unsaved-changes", false);
  }
};

const addRow = () => {
  const newRow = {
    orderLineId: 0,
    articleId: null,
    productName: "",
    quantity: 1,
    price: 0,
  };
  localOrderLines.push(newRow);
  detectUnsavedChanges();
  emit("update-order-lines", [...localOrderLines]);
  // console.log("Adding new row:", newRow);
};
// Removed createOrUpdateOrderLine() method that made direct API calls
const deleteOrderLine = (orderLineId, index) => {
  // If orderLineId is null, it’s a new row, so just remove from local
  if (!orderLineId) {
    localOrderLines.splice(index, 1);
    emit("update-order-lines", [...localOrderLines]);
    detectUnsavedChanges();
    return;
  }

  // Add the row to the deletedOrderLines array if it has an ID (i.e., it exists in the database)
  deletedOrderLines.push(localOrderLines[index]);

  // Remove the row from the localOrderLines array
  localOrderLines.splice(index, 1);

  // Emit the updated order lines to the parent
  emit("update-order-lines", [...localOrderLines]);

  // Emit the deleted order lines to the parent
  emit("deleted-order-lines", [...deletedOrderLines]); // Use spread operator to avoid Proxy issues

  // Track unsaved changes
  detectUnsavedChanges();

  // console.log("Deleted Order Line:", localOrderLines[index]);
  // console.log("Deleted Order Lines Array:", deletedOrderLines);
};

const updateLocalOrderLine = (orderLine, index) => {
  localOrderLines[index] = { ...orderLine };
  // Also emit updated lines to parent if needed
  emit("update-order-lines", [...localOrderLines]);
};

const isProductAlreadyOrdered = (productId) => {
  return localOrderLines.some((line) => line.articleId === productId);
};

/** (Optional) Validate an order line */
// const validateOrderLine = (orderLine) => {
//   if (!orderLine.orderId) {
//     console.error("OrderId is required.");
//     return false;
//   }
//   if (!orderLine.articleId) {
//     console.error("ArticleId is required.");
//     return false;
//   }
//   if (!orderLine.productName || orderLine.productName.trim() === "") {
//     console.error("ProductName is required.");
//     return false;
//   }
//   if (!orderLine.quantity || orderLine.quantity <= 0) {
//     console.error("Quantity must be > 0.");
//     return false;
//   }
//   return true;
// };

const openModal = (index) => {
  // console.log("test the open modal", index);
  selectedRowIndex.value = index;
  showModal.value = true;
  fetchArticles();
};

const handleArticleSelect = (article) => {
  if (selectedRowIndex.value !== null) {
    // Check if the product is already ordered
    if (isProductAlreadyOrdered(article.articleId)) {
      toast.error(
        "This product is already ordered. You cannot order the same product again."
      );
      return; // Exit the function if the product is already ordered
    }

    const selectedRow = localOrderLines[selectedRowIndex.value];
    selectedRow.articleId = article.articleId;
    selectedRow.productName = article.name;
    selectedRow.price = article.price;

    emit("update-order-lines", [...localOrderLines]);
    localOrderLines.splice(selectedRowIndex.value, 1, { ...selectedRow });
    // console.log("handleArticleSelect  -> selectedRow", selectedRow);
  }
  closeModal();
};

const closeModal = () => {
  showModal.value = false;
  selectedRowIndex.value = null;
};

const handleStay = () => {
  showWarningModal.value = false;
};

const handleLeave = () => {
  hasUnsavedChanges.value = false;
  showWarningModal.value = false;
  // You could navigate away or reload
  window.location.reload();
};

const beforeUnloadHandler = (event) => {
  if (hasUnsavedChanges.value) {
    event.preventDefault();
    event.returnValue = "";
    showWarningModal.value = true;
  }
};

// Pagination methods
const prevPage = () => {
  if (currentPage.value > 1) {
    currentPage.value--;
  }
};

const nextPage = () => {
  if (currentPage.value < totalPages.value) {
    currentPage.value++;
  }
};

onMounted(() => {
  window.addEventListener("beforeunload", beforeUnloadHandler);
});

onBeforeUnmount(() => {
  window.removeEventListener("beforeunload", beforeUnloadHandler);
});
</script>

// w-full border-collapse: Full-width responsive table with collapsed borders. // Header
rows: bg-blue-900 text-white p-2 text-center for consistent alignment and spacing. // Body
rows: border-t border-gray-300 for row separation. // Input Fields: // w-full p-2 border
border-gray-300 rounded: Full-width inputs with padding, borders, and rounded corners. //
focus:outline-none focus:ring focus:ring-blue-300: Adds focus effects. // Search Icon in
Input: // relative container and absolute icon positioning with right-2 top-1/2 transform
-translate-y-1/2 for perfect centering. // Add Row Button: // mb-2 mt-6 bg-blue-900
text-white px-4 py-2 rounded hover:bg-blue-700: Styled button with hover effect and margin
adjustments. // Action Button: // bg-red-600 text-white px-3 py-1 rounded
hover:bg-red-500: Red delete button with hover effects. // Responsiveness: // Tailwind
classes (like w-full, flex, and p-2) ensure the layout adjusts seamlessly across all
devices. // Table adapts to smaller screens, and the inputs remain accessible and
user-friendly.
