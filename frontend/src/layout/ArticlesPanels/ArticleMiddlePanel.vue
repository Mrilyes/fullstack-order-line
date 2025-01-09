<template>
  <div class="create-form">
    <h2 v-if="!isCreateMode && !selectedItem">
      <i class="fa-solid fa-database"></i>
      Article Management
    </h2>
    <h3 v-else>
      <i class="fa-solid fa-database"></i>
      {{ isCreateMode ? "Create Article" : "Update Article" }}
    </h3>
    <form @submit.prevent="isCreateMode ? emitCreate() : emitUpdate()">
      <div class="myform">
        <div class="form-group">
          <label for="name">Name</label>
          <input
            id="name"
            type="text"
            @input="trackChanges"
            :disabled="isInputsDisabled"
            v-model="localForm.name"
            placeholder="Enter article name"
            required
          />
        </div>
        <div class="form-group">
          <label for="price">Price</label>
          <input
            id="price"
            type="number"
            @input="trackChanges"
            :disabled="isInputsDisabled"
            v-model.number="localForm.price"
            placeholder="Enter article price"
            required
          />
        </div>
      </div>
    </form>
  </div>
</template>

<script>
export default {
  name: "ArticleMiddlePanel",
  props: {
    selectedItem: {
      type: Object,
      default: () => ({}),
    },
    isCreateMode: {
      type: Boolean,
      default: false,
    },
  },
  data() {
    return {
      localForm: { ...this.selectedItem },
      originalForm: { ...this.selectedItem },
      hasChanges: false, // Track whether the form has changes
    };
  },
  computed: {
    isInputsDisabled() {
      // Disable inputs when neither create mode is active nor a row is selected
      // The Object.keys check is only performed when selectedItem is defined and valid.
      return (
        !this.isCreateMode &&
        (!this.selectedItem || Object.keys(this.selectedItem).length === 0)
      );
    },
  },
  watch: {
    selectedItem: {
      handler(newValue) {
        this.localForm = { ...newValue };
        this.originalForm = { ...newValue };
        this.hasChanges = false; // Reset changes when a new item is selected
      },
      deep: true,
    },
  },
  methods: {
    emitCreate() {
      if (this.hasChanges) {
        this.$emit("create", this.localForm);
        this.localForm = {};
        this.hasChanges = false;
      } else {
        this.$emit("no-changes", "No changes to create");
      }
    },
    emitUpdate() {
      if (this.hasChanges) {
        this.$emit("update", this.localForm);
        this.originalForm = { ...this.localForm }; // Update the original form to reflect the new changes
        this.hasChanges = false;
      } else {
        this.$emit("no-changes", "No changes to update");
      }
    },
    clearForm() {
      this.localForm = {};
      this.hasChanges = false;
    },
    trackChanges() {
      // Check if the local form has diverged from the original form
      this.hasChanges =
        JSON.stringify(this.localForm) !== JSON.stringify(this.originalForm);
    },
    revertForm(originalData) {
      if (!originalData) {
        console.error("No original data provided to revert to.");
        return;
      }
      this.localForm = { ...originalData }; // Update the form fields
      this.hasChanges = false; // Mark as no changes
    },
  },
};
</script>

<style scoped>
.myform {
  width: 100%;
  height: 100%;
  padding: 1.5rem;
  background-color: #ffffff;
  display: flex;
  flex-direction: column;
  box-sizing: border-box;
  gap: 1rem;
  font-family: "Arial", sans-serif;
  color: #333;

  box-shadow: 0px 2px 5px rgba(0, 0, 0, 0.1);
}

/* tab */
.tabs {
  display: flex;
  /* border-bottom: 2px solid #e0e0e0; */
  margin-bottom: 1rem;
}

.tab-button {
  flex: 1;
  padding: 0.75rem 1rem;
  border: 1px solid #ccc;
  background-color: #bababa;
  cursor: pointer;
  text-align: center;
  font-weight: bold;
  transition: background-color 0.3s, color 0.3s;
}
.tab-button.active {
  background-color: #004085;
  color: white;
  /* border-bottom: 2px solid #007bff; */
}

/* Tab Content */

label {
  font-weight: bold;
  display: block;
  margin-bottom: 0.5rem;
}
/*
input {
  width: 20px;
  padding: 0.8rem;
  border: 1px solid #ccc;
  border-radius: 6px;
  font-size: 1rem;
  transition:
    border-color 0.3s,
    box-shadow 0.3s;
} */

input:focus {
  border-color: #004085;
  box-shadow: 0 0 8px rgba(0, 123, 255, 0.2);
  outline: none;
}

/* Form Rows */
.form-row {
  display: flex;
  flex-wrap: wrap;
  justify-content: space-around;
  gap: 1rem; /* Adjustable spacing */
  /* align-items: center; */
  /* margin-bottom: 1rem; */
}

.form-group {
  flex: 1; /* Inputs take equal space */
  min-width: 45%; /* Minimum width for each input in the row */
  display: flex;
  flex-direction: column;
}

.form-group label {
  font-weight: bold;
  margin-bottom: 0.1rem;
}

.form-group input {
  padding: 0.75rem;
  border: 1px solid #ccc;
  border-radius: 6px;
  font-size: 1rem;
  transition: border-color 0.3s, box-shadow 0.3s;
  flex: 1; /* Input height grows with the container */
}

.form-group input:focus {
  border-color: #004085;
  box-shadow: 0 0 8px rgba(0, 123, 255, 0.2);
  outline: none;
}

.header {
  font-size: 1.8rem;
  /* color: #007bff; */
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 1rem;
  /* padding-bottom: 1rem; */
}

svg {
  color: #004085;
  font-size: 2rem;
  /* padding: 10px; */
  /* background-color: #004085; */
}

form {
  margin-top: 1rem;
}

button {
  margin-top: 1rem;
}

label {
  margin-top: 0.5rem;
  font-weight: bold;
}

label:after {
  content: "*";
  color: red;
  margin-left: 5px;
  font-size: 0.8rem;
}

input {
  margin-bottom: 1rem;
  padding: 0.5rem;
  width: 100%;
  box-sizing: border-box;
  border: 1px solid black;
  border-radius: 2px;
  display: block;
}

input:disabled {
  background-color: #f0f0f0;
  cursor: not-allowed;
  opacity: 0.5;
}

button {
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 5px;
  padding: 0.5rem;
  cursor: pointer;
}

button:hover {
  background-color: #0056b3;
}

button:active {
  background-color: #004085;
}
</style>
