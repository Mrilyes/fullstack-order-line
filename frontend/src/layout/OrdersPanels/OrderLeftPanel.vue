<template>
  <div>
    <!-- <h3 class="text-3xl flex items-center gap-2">Order Table</h3> -->
    <DataTable
      :data="transformedData"
      :columns="columns"
      :selectedRow="transformedSelectedRow"
      @row-selected="handleRowSelected"
      @page-changed="handlePageChange"
      @sort-changed="handleSortChange"
      :loading="loading"
      :currentPage="currentPage"
      :totalPages="totalPages"
    />
  </div>
</template>

<script setup>
import DataTable from "@/components/DataTable.vue";
import apiServices from "@/services/apiServices";
import { ref, computed, onMounted } from "vue";

const props = defineProps({
  apiUrl: {
    type: String,
    required: true,
  },
  selectedRow: {
    type: Object,
    default: null,
  },
});

const emit = defineEmits(["row-selected"]); // Emit event for row selection

const columns = [
  // { field: "orderNumber", header: "Order Number" },
  { field: "customerName", header: "Customer Name" },
  { field: "orderDate", header: "Order Date" },
];

const tableData = ref([]); // Mock data
const currentPage = ref(1);
const sortField = ref(null);
const sortOrder = ref(null);
const loading = ref(false); // Loading state
const totalPages = ref(1); // Total number of pages

// Transform table data to include a mapped 'id' field for DataTable compatibility
const transformedData = computed(() => {
  return tableData.value.map((item) => ({
    ...item,
    // Adjusted Data Mapping: Ensured that each data item has a unique id by mapping orderId to id.
    id: item.orderId || item.id || Math.random(), // Ensure 'id' is defined
    // Map articleId to id for DataTable
  }));
});

// Transform selectedRow to match the transformedData for DataTable compatibility
const transformedSelectedRow = computed(() => {
  return props.selectedRow
    ? {
        ...props.selectedRow,
        // Adjusted Data Mapping: Ensured that each data item has a unique id by mapping orderId to id.
        id: props.selectedRow.orderId || props.selectedRow.id || Math.random(),
      }
    : null;
});

const handleRowSelected = (row) => {
  const originalRow = tableData.value.find(
    // Adjusted Data Mapping: Ensured that each data item has a unique id by mapping orderId to id.
    (item) => item.orderId === row.id || item.id === row.id
  );
  emit("row-selected", originalRow); // Emit the original row
};

const fetchTableData = async (page = 1, sort = {}) => {
  loading.value = true;
  try {
    // Construct query parameters based on page and sort
    const params = {
      page,
      limit: 4, // Assuming 4 rows per page
      sortField: sort.field || "",
      sortOrder: sort.order || "",
    };
    const response = await apiServices.get(props.apiUrl, { params });
    // console.log("Fetched data:", response);
    tableData.value = response; // Adjust based on your API's response structure
    // Optionally, handle total pages if your API provides it
    // Also, if the API returns total pages, set it accordingly
    if (response.totalPages) {
      totalPages.value = response.totalPages;
    } else {
      // If totalPages is not provided, calculate based on totalItems if available
      if (response.totalItems) {
        totalPages.value = Math.ceil(response.totalItems / params.limit);
      } else {
        // Default to 1 if totalPages cannot be determined
        totalPages.value = 1;
      }
    }
  } catch (error) {
    console.error("Error fetching order data:", error);
    tableData.value = [];
    totalPages.value = 1; // Reset to default if fetch fails
  } finally {
    loading.value = false;
  }
};

const handlePageChange = (newPage) => {
  currentPage.value = newPage;
  fetchTableData(newPage, { field: sortField.value, order: sortOrder.value });
};

const handleSortChange = ({ field, order }) => {
  sortField.value = field;
  sortOrder.value = order;
  fetchTableData(currentPage.value, { field, order });
};

onMounted(() => {
  fetchTableData(); // Fetch initial data on component mount
});

// Expose methods to template refs
defineExpose({
  fetchTableData,
});
</script>
