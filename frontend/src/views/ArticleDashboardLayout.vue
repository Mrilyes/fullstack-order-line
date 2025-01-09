<template>
  <div class="dashboard">
    <Navbar />
    <div class="main-content">
      <div class="resizable-panels">
        <splitpanes class="default-theme" :horizontal="false">
          <!-- Left Panel -->
          <pane>
            <div class="left-panel" style="background-color: #ffff">
              <ArticleLeftPanel
                :apiUrl="articleApiUrl"
                :selectedRow="selectedRow"
                @row-selected="onRowSelected"
                ref="articleLeftPanel"
              />
            </div>
          </pane>
          <!-- Middle Panel -->
          <pane>
            <div class="middle-panel" style="background-color: #ffff">
              <ArticleMiddlePanel
                :selectedItem="selectedRow"
                :isCreateMode="isCreating"
                @create="handleCreate"
                @update="handleUpdate"
                ref="articleMiddlePanel"
              />
            </div>
          </pane>
        </splitpanes>
        <!-- Right Panel -->
        <div class="right-panel" style="background-color: #ffff">
          <ArticleRightPanel
            :isCreateMode="isCreating"
            :hasSelectedRow="!!selectedRow"
            @create="startCreating"
            @save="saveChanges"
            @undo="undoChanges"
            @refresh="refreshLeftPanel"
            @delete="deleteRow"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import Navbar from "@/components/AppNavbar.vue";
import ArticleLeftPanel from "@/layout/ArticlesPanels/ArticleLeftPanel.vue";
import ArticleMiddlePanel from "@/layout/ArticlesPanels/ArticleMiddlePanel.vue";
import ArticleRightPanel from "@/layout/ArticlesPanels/ArticleRightPanel.vue";
import apiServices from "@/services/apiServices";
import { Splitpanes, Pane } from "splitpanes";
import "splitpanes/dist/splitpanes.css";
import { useToast } from "vue-toastification";

export default {
  name: "DashboardLayout",
  components: {
    Navbar,
    ArticleLeftPanel,
    ArticleMiddlePanel,
    ArticleRightPanel,
    Splitpanes,
    Pane,
  },
  data() {
    return {
      articleApiUrl: "https://localhost:7128/api/Article",
      selectedRow: null, // Holds the currently selected row in the DataTable
      isCreating: false, // Determines whether we are in Create Mode
      isInitialState: true, // Add to track initial state
    };
  },
  setup() {
    const toast = useToast(); // Initialize toast notifications
    return { toast };
  },
  methods: {
    async refreshLeftPanel() {
      try {
        await this.$refs.articleLeftPanel.fetchTableData(); // Call fetch method in Left Panel
        this.toast.success("Table refreshed successfully!");
      } catch (error) {
        console.error("Error refreshing table:", error);
        this.toast.error("Failed to refresh table.");
      }
    },
    onRowSelected(rowData) {
      this.isInitialState = false; // No longer in initial state
      this.selectedRow = { ...rowData }; // Update the selected row
      this.isCreating = false; // Switch to Update Mode
      console.log("Dashboard selectedRow:", this.selectedRow); // Debug log

      this.toast.info("Row selected for editing.");
    },
    startCreating() {
      this.isInitialState = false; // No longer in initial state
      this.selectedRow = null; // Clear the selection
      this.isCreating = true; // Switch to Create Mode
      this.toast.info("Switched to create mode.");
    },
    async handleCreate(newItem) {
      // Preprocess input to normalize strings (e.g., trim spaces)
      const processedItem = this.preprocessItem(newItem);

      // Check for duplication
      //
      const existingItems = await apiServices.get(this.articleApiUrl);

      // Check for duplicate items (excluding the currently selected item)
      const isDuplicate = existingItems.some(
        (item) =>
          item.name.toLowerCase() === processedItem.name.toLowerCase()
      );

      if (isDuplicate) {
        this.toast.error("The item already exists.");
        console.error("Duplicate entry detected.");
        return; // Exit without sending the API request
      }
      try {
        await apiServices.post(this.articleApiUrl, newItem);
        await this.refreshLeftPanel();
        this.toast.success("Item created successfully!");
      } catch (error) {
        console.error("Error creating item:", error);
        this.toast.error("Failed to create item.");
      }
    },
    preprocessItem(item) {
      // Create a copy of the item and trim all string fields
      return Object.keys(item).reduce((acc, key) => {
        acc[key] = typeof item[key] === "string" ? item[key].trim() : item[key];
        return acc;
      }, {});
    },
    async handleUpdate(updatedItem) {
      // Preprocess both the updated item and the selected row to remove whitespace
      const processedUpdatedItem = this.preprocessItem(updatedItem);
      const processedSelectedRow = this.preprocessItem(this.selectedRow);

      if (JSON.stringify(processedUpdatedItem) === JSON.stringify(processedSelectedRow)) {
        this.toast.info("No changes detected. The item is already up-to-date.");
        return; // Exit without sending the request
      }

      try {
        // Fetch all existing items to check for duplicates
        const existingItems = await apiServices.get(this.articleApiUrl);

        // Check for duplicate items (excluding the currently selected item)
        const isDuplicate = existingItems.some(
          (item) =>
            item.id !== processedSelectedRow.id && // Exclude the current item
            item.name.trim().toLowerCase() ===
              processedUpdatedItem.name.trim().toLowerCase()
        );

        if (isDuplicate) {
          this.toast.error("The item already exists.");
          console.error("Duplicate entry detected.");
          // Reset the form to the original data
          this.$refs.articleMiddlePanel.revertForm(this.selectedRow);

          return; // Exit without sending the request
        }
        // Proceed with the update
        const updateUrl = `${this.articleApiUrl}/${updatedItem.articleId}`;
        await apiServices.put(updateUrl, processedUpdatedItem);

        await this.refreshLeftPanel();
        this.toast.success("Item updated successfully!");
      } catch (error) {
        console.error("Error updating item:", error);
        this.toast.error("Failed to update item.");
      }
    },
    async deleteRow() {
      if (!this.selectedRow) return; // Ensure there's a selected row
      try {
        const deleteUrl = `${this.articleApiUrl}/${this.selectedRow.articleId}`;
        await apiServices.delete(deleteUrl);
        this.selectedRow = null; // Clear the selection
        await this.refreshLeftPanel();
        this.toast.success("Item deleted successfully!");
      } catch (error) {
        console.error("Error deleting row:", error);
        this.toast.error("Failed to delete item.");
      }
    },
    undoChanges() {
      if (this.isCreating) {
        // Clear form inputs
        this.$refs.articleMiddlePanel.clearForm();
        this.toast.info("Create Changes undone.");
      } else {
        // If no recent save exists, revert to the initial selected row
        // this.$refs.middlePanel.revertForm(this.selectedRow)
        // If no recent save exists, revert to the initial selected row
        const previousRowId = this.selectedRow?.id; // Store the current selected row ID
        try {
          // Find the updated row in the table after refresh
          const updatedRow = this.$refs.articleLeftPanel.tableData.find(
            (row) => row.id === previousRowId
          );
          if (updatedRow) {
            this.selectedRow = { ...updatedRow }; // Update the selected row with the latest state
            this.$refs.articleMiddlePanel.revertForm(this.selectedRow); // Revert the form to the latest state
            this.toast.info("Reverted to the latest saved data.");
          } else {
            this.toast.error("Unable to find the row after refreshing.");
          }
        } catch (error) {
          console.error("Error refreshing table data:", error);
          this.toast.error("Failed to refresh table data.");
        }
        // this.toast.info('Reverted to initial data.')
      }
    },
    saveChanges() {
      if (this.isCreating) {
        this.$refs.articleMiddlePanel.emitCreate(); // Trigger create
      } else {
        this.$refs.articleMiddlePanel.emitUpdate(); // Trigger update
      }
    },
  },
};
</script>

<style scoped>
.main-content {
  display: flex;
  flex-direction: column;
  height: 100vh;
}

.resizable-panels {
  display: flex;
  flex: 1;
}

.left-panel,
.middle-panel,
.right-panel {
  flex: 1;
  background-color: #ffffff;
  padding: 1rem;
  height: 100%;
  box-sizing: border-box;
}

::v-deep(.splitpanes__splitter) {
  background-color: #003f7f;
  width: 5px;
  cursor: col-resize;
}

::v-deep(.splitpanes__splitter:hover) {
  background-color: #003f7f;
}
</style>
