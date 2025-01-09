<template>
  <div
    class="p-6 bg-white shadow-md flex flex-col gap-4 text-gray-800 font-sans max-w-7xl mx-auto"
  >
    <!-- Header Section -->
    <h2
      v-if="!isCreateMode && !selectedItem"
      class="text-2xl md:text-3xl flex items-center gap-2"
    >
      <i class="fa-solid fa-database text-blue-800"></i>
      Orders Management
    </h2>
    <h3 v-else class="text-2xl md:text-3xl flex items-center gap-2">
      <i class="fa-solid fa-database text-blue-800"></i>
      {{ isCreateMode ? "Create Order" : "Update Order" }}
    </h3>

    <!-- Form Section -->
    <form @submit.prevent="isCreateMode ? emitCreate() : emitUpdate()">
      <div v-if="activeTab === 'main'" class="w-full flex flex-col gap-4">
        <div class="flex flex-wrap md:flex-nowrap justify-between gap-4">
          <!-- Customer Name -->
          <div class="flex-1 min-w-[100%] md:min-w-[45%] flex flex-col">
            <label for="customerName" class="font-bold mb-1"
              >Customer Name<span class="text-red-500">*</span></label
            >
            <input
              id="customerName"
              type="text"
              v-model="localForm.customerName"
              placeholder="Name"
              @input="trackChanges"
              :disabled="isInputsDisabled"
              class="p-3 border border-gray-300 rounded-md focus:outline-none focus:ring focus:ring-blue-300 disabled:bg-gray-100 disabled:cursor-not-allowed disabled:opacity-50"
              required
            />
          </div>

          <!-- Order Date -->
          <div class="flex-1 min-w-[100%] md:min-w-[45%] flex flex-col">
            <label for="orderDate" class="font-bold mb-1"
              >Order Date<span class="text-red-500">*</span></label
            >
            <input
              id="orderDate"
              type="date"
              v-model="localForm.orderDate"
              @input="trackChanges"
              :disabled="isInputsDisabled"
              class="p-3 border border-gray-300 rounded-md focus:outline-none focus:ring focus:ring-blue-300 disabled:bg-gray-100 disabled:cursor-not-allowed disabled:opacity-50"
              required
            />
          </div>
        </div>
      </div>
    </form>

    <!-- Order Lines Table -->
    <div class="mt-6 overflow-x-auto">
      <OrderLinesTable
        :orderLines="localOrderLines"
        :orderId="localForm.orderId"
        @update-order-lines="updateOrderLines"
        @deleted-order-lines="handleDeletedOrderLines"
        @open-modal="openModalForProduct"
      />
    </div>

    <!-- Article Selection Modal -->
    <ArticleSelectionModal
      v-if="isModalVisible"
      :visible="isModalVisible"
      :products="articles"
      @product-selected="handleProductSelected"
      @close="closeModal"
    />
  </div>
</template>

<script setup>
import OrderLinesTable from "@/components/OrderLinesTable.vue";
import ArticleSelectionModal from "@/components/ArticleSelectionModal.vue";
import apiServices from "@/services/apiServices";

import { ref, computed, watch, onMounted, onBeforeUnmount } from "vue";

const props = defineProps({
  selectedItem: {
    type: Object,
    default: () => ({}),
  },
  isCreateMode: {
    type: Boolean,
    default: false,
  },
  orderLines: {
    type: Array,
    default: () => [],
  },
});
// Emits
const emit = defineEmits([
  "create",
  "update",
  "no-changes",
  "unsaved-changes",
  "update-order-lines",
  "deleted-order-lines",
]);

// State
const activeTab = ref("main");
const localForm = ref({ ...props.selectedItem });
const originalForm = ref({ ...props.selectedItem });
const localOrderLines = ref(JSON.parse(JSON.stringify(props.orderLines)));
const hasChanges = ref(false);
const isModalVisible = ref(false);
const articles = ref([]);
const currentEditingRowIndex = ref(null);
const showWarningModal = ref(false);
const hasUnsavedChanges = ref(false);
const deletedOrderLines = ref([]);

// computed
const isInputsDisabled = computed(() => {
  return (
    !props.isCreateMode &&
    (!props.selectedItem || Object.keys(props.selectedItem).length === 0)
  );
});

// watches
watch(
  () => props.selectedItem,
  (newValue) => {
    localForm.value = { ...newValue };
    originalForm.value = { ...newValue };
    hasChanges.value = false;
  },
  { deep: true }
);

watch(
  () => props.orderLines,
  (newValue) => {
    localOrderLines.value = JSON.parse(JSON.stringify(newValue));
  },
  { deep: true }
);

const fetchArticles = async () => {
  try {
    articles.value = await apiServices.get("https://localhost:7128/api/Article");
  } catch (error) {
    console.error("Error fetching articles:", error);
  }
};

const openModalForProduct = (rowIndex) => {
  currentEditingRowIndex.value = rowIndex;
  isModalVisible.value = true;
  fetchArticles();
};

// const handleUnsavedChanges = (hasChanges) => {
//   console.log("Unsaved changes from lines table:", hasChanges);
//   hasUnsavedChanges.value = hasChanges;
//   emit("unsaved-changes", hasChanges);
// };

const detectUnsavedChanges = () => {
  // console.log("Unsaved changes detected in middle panel.");
  hasUnsavedChanges.value = true;
  emit("unsaved-changes", true);
};

const trackChanges = () => {
  hasChanges.value =
    JSON.stringify(localForm.value) !== JSON.stringify(originalForm.value);
  if (hasChanges.value) {
    detectUnsavedChanges();
  }
};

const handleProductSelected = (product) => {
  if (currentEditingRowIndex.value !== null) {
    const updatedOrderLines = [...localOrderLines.value];
    updatedOrderLines[currentEditingRowIndex.value] = {
      ...updatedOrderLines[currentEditingRowIndex.value],
      articleId: product.articleId,
      productName: product.name,
      price: product.price,
    };
    localOrderLines.value = updatedOrderLines;
    updateOrderLines.value(updatedOrderLines);
    detectUnsavedChanges();
    closeModal();
  }
};

const handleDeletedOrderLines = (deletedLines) => {
  deletedOrderLines.value = deletedLines;
  // console.log("Deleted Order Lines in Middle Panel:", deletedOrderLines.value);
  emit("deleted-order-lines", [...deletedOrderLines.value]); // Use spread operator to unwrap Proxy

  detectUnsavedChanges();
};

const closeModal = () => {
  isModalVisible.value = false;
  currentEditingRowIndex.value = null;
};

const emitCreate = () => {
  if (hasChanges.value || hasUnsavedChanges.value) {
    emit("create", {
      ...localForm.value,
      orderLines: localOrderLines.value,
      deletedOrderLines: deletedOrderLines.value, // Include deleted lines
    });
    localForm.value = {};
    localOrderLines.value = [];
    deletedOrderLines.value = []; // Clear deleted lines
    hasChanges.value = false;
    hasUnsavedChanges.value = false;
  } else {
    emit("no-changes", "No changes to create");
  }
};

const emitUpdate = () => {
  if (hasChanges.value || hasUnsavedChanges.value) {
    emit("update", {
      ...localForm.value,
      orderLines: localOrderLines.value,
      deletedOrderLines: deletedOrderLines.value, // Include deleted lines
    });
    originalForm.value = { ...localForm.value };
    deletedOrderLines.value = []; // Clear deleted lines
    hasChanges.value = false;
    hasUnsavedChanges.value = false;
  } else {
    emit("no-changes", "No changes to update");
  }
};

const updateOrderLines = (updatedLines) => {
  localOrderLines.value = updatedLines;
  emit("update-order-lines", updatedLines);
  // console.log("update order line ", updatedLines);
  detectUnsavedChanges();
};

const clearForm = () => {
  localForm.value = {};
  hasChanges.value = false;
};

const beforeUnloadHandler = (event) => {
  if (hasUnsavedChanges.value) {
    event.preventDefault();
    event.returnValue = "";
    showWarningModal.value = true;
  }
};

onMounted(() => {
  window.addEventListener("beforeunload", beforeUnloadHandler);
});

onBeforeUnmount(() => {
  window.removeEventListener("beforeunload", beforeUnloadHandler);
});

// Expose methods to template refs
defineExpose({
  clearForm,
  updateOrderLines,
  emitCreate,
  emitUpdate,
});
</script>
