<template>
  <div class="flex flex-wrap gap-4">
    <ActionButtons :buttons="buttonsConfig" @button-clicked="handleButtonClick" />

    <!-- Delete Confirmation Modal -->
    <ConfirmationModal
      title="Confirm Delete"
      message="Are you sure you want to delete this order?"
      @confirm="confirmDelete"
      ref="deleteModal"
    />
  </div>
</template>

<script setup>
import { ref, computed, defineProps, defineEmits } from "vue";
import ActionButtons from "@/components/ActionButtons.vue";
import ConfirmationModal from "@/components/ConfirmationModal.vue";

// -- Define Props --
const props = defineProps({
  isCreateMode: {
    type: Boolean,
    default: false,
  },
  hasSelectedRow: {
    type: Boolean,
    default: false,
  },
  isInitialState: {
    type: Boolean,
    default: true,
  },
  placeholder: { type: String, default: "" },
});

// -- Define Emits --
const emit = defineEmits(["create", "save", "undo", "refresh", "delete"]);

// -- Template ref to ConfirmationModal --
const deleteModal = ref(null);

// -- Computed: buttons configuration --
const buttonsConfig = computed(() => [
  {
    icon: "fas fa-plus",
    action: "create",
    disabled: props.isCreateMode,
    customStyle: "!bg-orange-600 text-white", // Tailwind classes
    placeholder: "Create",
  },
  {
    icon: "fas fa-save",
    action: "save",
    disabled: !props.isCreateMode && !props.hasSelectedRow,
    placeholder: "Save",
  },
  {
    icon: "fas fa-trash",
    action: "delete",
    disabled: !props.hasSelectedRow,
    placeholder: "Delete",
  },
  {
    icon: "fas fa-undo",
    action: "undo",
    disabled: !props.isCreateMode && !props.hasSelectedRow,
    placeholder: "Undo",
  },
  {
    icon: "fas fa-sync-alt",
    action: "refresh",
    disabled: false,
    placeholder: "Refresh",
  },
  {
    icon: "fas fa-print",
    action: "print",
    disabled: false,
    placeholder: "Print",
  },
]);

// -- Methods --
const handleButtonClick = (action) => {
  if (action === "delete") {
    // Show confirmation modal
    deleteModal.value?.showModal();
  } else {
    // Emit other actions
    emit(action);
  }
};

const confirmDelete = () => {
  // Emit delete event to parent
  emit("delete");
};
</script>

<style scoped>
.right-panel {
  padding: 1rem;
}
</style>
