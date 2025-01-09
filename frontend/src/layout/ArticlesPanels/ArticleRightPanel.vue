<template>
  <ActionButtons :buttons="buttonsConfig" @button-clicked="handleButtonClick" />

  <!-- Include ConfirmationModal -->
  <ConfirmationModal
    modalId="deleteConfirmationModal"
    title="Confirm Delete"
    message="Are you sure you want to delete this item?"
    @confirm="confirmDelete"
    ref="deleteModal"
  />
</template>
<script>
import ActionButtons from '@/components/ActionButtons.vue'
import ConfirmationModal from '@/components/ConfirmationModal.vue'

export default {
  name: 'ArticleRightPanel',
  components: { ActionButtons, ConfirmationModal },
  emits: ['create', 'save', 'delete', 'undo', 'refresh', 'print'],
  props: {
    isCreateMode: {
      type: Boolean,
      default: false,
    },
    hasSelectedRow: {
      type: Boolean,
      default: false,
    },
    isInitialState: {
      // New prop to determine if the dashboard is in its initial state
      type: Boolean,
      default: true,
    },
    placeholder: {
      type: Text,
    },
  },
  computed: {
    buttonsConfig() {
      return [
        {
          // label: 'Create',
          icon: 'fas fa-plus',
          action: 'create',
          placeholder: 'Create',
          disabled: this.isCreateMode,
          customStyle: { backgroundColor: '#f18305', color: 'white' }, // Orange background
          initialState: true, // Enabled in initial state
        },
        {
          // label: 'Save',
          icon: 'fas fa-save',
          action: 'save',
          placeholder: 'Save',
          disabled: !this.isCreateMode && !this.hasSelectedRow,
          initialState: false,
        },
        {
          // label: 'Delete',
          icon: 'fa-solid fa-trash-can',
          action: 'delete',
          placeholder: 'Delete',
          disabled: !this.hasSelectedRow,
          initialState: false,
        },
        {
          // label: 'Undo',
          icon: 'fa-solid fa-x',
          action: 'undo',
          placeholder: 'Undo',
          disabled: !this.isCreateMode && !this.hasSelectedRow,
          initialState: false,
        },
        // Label: "Refresh"
        {
          icon: 'fas fa-sync-alt',
          action: 'refresh',
          placeholder: 'Refresh',
          disabled: false,
        },
        // Label : "Print"
        {
          icon: 'fas fa-print',
          action: 'print',
          placeholder: 'Print',
          disabled: false,
          initialState: false,
        },
      ]
    },
  },
  methods: {
    handleButtonClick(action) {
      if (action === 'delete') {
        this.$refs.deleteModal.showModal()
      } else {
        this.$emit(action)
      }
    },
    confirmDelete() {
      this.$emit('delete')
    },
  },
}
</script>

<style scoped>
.right-panel {
  padding: 1rem;
}
</style>
