<template>
  <div class="dashboard">
    <Navbar />
    <div class="main-content flex flex-col h-screen">
      <div class="resizable-panels flex flex-1">
        <splitpanes class="default-theme" :horizontal="false">
          <!-- Left Panel -->
          <pane :size="40">
            <div class="left-panel flex-1 bg-white p-4 h-full box-border">
              <OrderLeftPanel
                :apiUrl="orderApiUrl"
                :selectedRow="selectedRow"
                @row-selected="onRowSelected"
                ref="orderLeftPanel"
              />
            </div>
          </pane>
          <!-- Middle Panel -->
          <pane>
            <div class="middle-panel flex-1 bg-white p-4 h-full box-border">
              <OrderMiddlePanel
                :selectedItem="selectedRow"
                :orderLines="orderLines"
                :isCreateMode="isCreating"
                :isAddButtonDisable="isAddButtonDisable"
                @create="handleCreate"
                @update="handleUpdate"
                @deleted-order-lines="handleDeletedOrderLines"
                @unsaved-changes="handleUnsavedChanges"
                ref="orderMiddlePanel"
              />
            </div>
          </pane>
        </splitpanes>
        <!-- Right Panel -->
        <div class="right-panel flex-1 bg-white p-4 h-full box-border">
          <OrderRightPanel
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
    <WarningModal :visible="showWarningModal" @stay="handleStay" @leave="handleLeave" />
  </div>
</template>

<script setup>
import Navbar from "@/components/AppNavbar.vue";
import OrderLeftPanel from "@/layout/OrdersPanels/OrderLeftPanel.vue";
import OrderMiddlePanel from "@/layout/OrdersPanels/OrderMiddlePanel.vue";
import OrderRightPanel from "@/layout/OrdersPanels/OrderRightPanel.vue";
import WarningModal from "@/components/WarningModal.vue";
import apiServices from "@/services/apiServices";
import { Splitpanes, Pane } from "splitpanes";
import "splitpanes/dist/splitpanes.css";
import { useToast } from "vue-toastification";
import { ref } from "vue";

// Refs for reactive state
const orderApiUrl = ref("https://localhost:7128/api/Order");
const selectedRow = ref(null);
const isCreating = ref(false);
const orderLines = ref([]);
const isInitialState = ref(true);
const showWarningModal = ref(false);
const hasUnsavedChanges = ref(false);
const pendingSelectedRow = ref(null);
const deletedOrderLines = ref([]);
const isAddButtonDisable = ref(false);

// Template refs
const orderLeftPanel = ref(null);
const orderMiddlePanel = ref(null);

// Toast
const toast = useToast();

// methods
const refreshLeftPanel = async () => {
  try {
    await orderLeftPanel.value.fetchTableData();
    // toast.success("Table refreshed successfully!");
  } catch (error) {
    console.error("Error refreshing table:", error);
    toast.error("Failed to refresh table.");
  }
};

const onRowSelected = (order) => {
  if (hasUnsavedChanges.value) {
    pendingSelectedRow.value = order;
    showWarningModal.value = true;
  } else {
    selectedRow.value = { ...order };
    isCreating.value = false;
    orderLines.value = order.orderLines || [];
    isAddButtonDisable.value = true;
    // toast.info("Order selected.");
  }
};

const startCreating = () => {
  if (hasUnsavedChanges.value) {
    showWarningModal.value = true;
  } else {
    isInitialState.value = false;
    selectedRow.value = null;
    orderLines.value = [];
    isCreating.value = true;
    isAddButtonDisable.value = true;
    // toast.info("Switched to create mode.");
  }
};

const handleUnsavedChanges = (hasChanges) => {
  hasUnsavedChanges.value = hasChanges;
};

const handleStay = () => {
  showWarningModal.value = false;
};

const handleLeave = () => {
  hasUnsavedChanges.value = false;
  showWarningModal.value = false;
  if (isCreating.value) {
    isCreating.value = false;
    selectedRow.value = null;
    orderLines.value = [];
  } else {
    selectedRow.value = { ...pendingSelectedRow.value };
    orderLines.value = selectedRow.value.orderLines || [];
  }
};

const preprocessOrder = (order) => {
  return {
    ...order,
    customerName: order.customerName.trim(),
    orderLines: (order.orderLines || []).map((line) => ({
      ...line,
      // Make sure quantity / price are numbers
      quantity: Number(line.quantity),
      price: Number(line.price),
    })),
  };
};

const handleCreate = async (newOrder) => {
  const processedOrder = preprocessOrder(newOrder);
  try {
    // Create the new order
    await apiServices.post(orderApiUrl.value, processedOrder);

    await refreshLeftPanel();
    toast.success("Order created successfully!");
    hasUnsavedChanges.value = false;
  } catch (error) {
    console.error("Error creating order:", error);
    toast.error("Failed to create order.");
  }
};

const handleUpdate = async (updatedOrder) => {
  const processedOrder = preprocessOrder(updatedOrder);
  const updateUrl = `${orderApiUrl.value}/${processedOrder.orderId}`;
  try {
    // Update the order
    await apiServices.put(updateUrl, processedOrder);

    await refreshLeftPanel();
    toast.success("Order updated successfully!");
    hasUnsavedChanges.value = false;
  } catch (error) {
    console.error("Error updating order:", error);
    toast.error("Failed to update order.");
  }
};

const handleDeletedOrderLines = (deletedLines) => {
  // Store the deleted lines for later use
  deletedOrderLines.value = deletedLines;
};

const deleteRow = async () => {
  if (!selectedRow.value) {
    toast.error("No order selected for deletion.");
    return;
  }
  try {
    const deleteUrl = `${orderApiUrl.value}/${selectedRow.value.orderId}`;
    await apiServices.delete(deleteUrl);
    selectedRow.value = null;
    orderLines.value = [];
    await refreshLeftPanel();
    toast.success("Order deleted successfully!");
  } catch (error) {
    console.error("Error deleting order:", error);
    toast.error("Failed to delete order.");
  }
};

const undoChanges = () => {
  if (isCreating.value) {
    orderMiddlePanel.value.clearForm();
    // toast.info("Create changes undone.");
  } else {
    selectedRow.value = { ...selectedRow.value };
    orderMiddlePanel.value.updateOrderLines(orderLines.value);
    // toast.info("Changes undone.");
  }
  hasUnsavedChanges.value = false;
};

const saveChanges = () => {
  // Single button that triggers create or update
  if (isCreating.value) {
    orderMiddlePanel.value.emitCreate();
  } else {
    orderMiddlePanel.value.emitUpdate();
  }
};
</script>
