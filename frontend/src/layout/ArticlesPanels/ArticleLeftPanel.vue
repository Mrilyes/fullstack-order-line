<template>
  <h3>Articles Management</h3>
  <DataTable
    :key="selectedRow"
    :data="transformedData"
    :columns="columns"
    :selectedRow="transformedSelectedRow"
    @row-selected="handleRowSelected"
  />
</template>

<script>
import DataTable from "@/components/DataTable.vue";
import apiServices from "@/services/apiServices";

export default {
  name: "ArticleLeftPanel",
  components: {
    DataTable,
  },
  props: {
    apiUrl: {
      type: String,
      required: true,
    },
    selectedRow: {
      type: Object,
      default: null,
    },
  },
  emits: ["row-selected"], // to explicitly declare the emitted event.

  data() {
    return {
      tableData: [],
      columns: [
        { field: "name", header: "Name" },
        { field: "price", header: "Price" },
        // { field: 'regionImageUrl', header: 'RegionImageUrl' },
      ],
    };
  },
  computed: {
    // Transform table data to include a mapped 'id' field for DataTable compatibility
    transformedData() {
      return this.tableData.map((item) => ({
        ...item,
        id: item.articleId, // Map articleId to id for DataTable
      }));
    },
    // Transform selectedRow to match the transformedData for DataTable compatibility
    transformedSelectedRow() {
      return this.selectedRow
        ? { ...this.selectedRow, id: this.selectedRow.articleId }
        : null;
    },
  },
  methods: {
    handleRowSelected(row) {
      const originalRow = this.tableData.find((item) => item.articleId === row.id);
      this.$emit("row-selected", originalRow); // Emit the original row
    },
    async fetchTableData() {
      // Use dummy data for now
      try {
        const response = await apiServices.get(this.apiUrl);
        console.log("Fetched data:", response); // Ensure unique IDs

        this.tableData = response;
        return response;
      } catch (error) {
        console.error("Error fetching order data:", error);
        this.tableData = [];
        throw error;
      }
    },
  },
  async mounted() {
    await this.fetchTableData(); // Fetch data when component is mounted
  },
};
</script>

<style scoped>
.left-panel {
  padding: 1rem;
}
</style>
