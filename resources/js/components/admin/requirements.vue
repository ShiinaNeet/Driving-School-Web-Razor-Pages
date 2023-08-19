<template>
    <div class="flex content-between">
        <p>Requirements</p>
        <va-button @click="isRequirementVisible = true">Create</va-button>
    </div>
    <div class="va-table-responsive">
        <table class="va-table">
            <thead>
                <tr>
                    <th>Name</th>
                    <th>Description</th>
                    <th>Created at</th>
                    <th>Action</th>
                </tr>
            </thead>
            <tbody>
                <tr
                v-for="requirement in requirements"
                :key="requirement.id"
                >
                    <td>{{ requirement.name }}</td>
                    <td>{{ requirement.description }}</td>
                    <td>{{ formatDate(requirement.created_at, 'MMM. DD, YYYY', 'N/A') }}</td>
                    <td>
                        <va-button @click="editRequirement(requirement)">Edit</va-button>&nbsp;
                        <va-button @click="deleteRequirement(requirement)">Delete</va-button>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    <va-modal
    v-model="isRequirementVisible"
    hide-default-actions
    >
        <h4 class="va-h4">
            {{ requirement.id ? 'Edit' : 'Create' }} requirement
        </h4>
        <div>
            <va-input
                v-model="requirement.name"
                type="text"
                label="Name"
                class="mb-2 w-full"
                outline
            />
            <va-input
                v-model="requirement.description"
                type="text"
                label="Description"
                class="mb-2 w-full"
                outline
            />
        </div>
        <template #footer>
            <va-button color="#FFFFFF" @click="isRequirementVisible = false, requirement = {}">Cancel</va-button>&nbsp;
            <va-button @click="saveRequirement">Save</va-button>
        </template>
  </va-modal>
</template>

<style lang="scss" scoped>
.va-table-responsive {
  overflow: auto;
}
</style>

<script>
import formatDate from '@/functions/formatdate.js';

export default {
    data () {
        return {
            isLoading: false,
            isDeleteLoading: false,
            isSaveLoading: false,
            isRequirementVisible: false,
            requirements: [],
            requirement: {}
        };
    },
    mounted() {
        this.getRequirements();
    },
    methods: {
        deleteRequirement(requirement) {
            this.$vaModal.init({
                message: 'Are you sure you want to delete ' + requirement.name + '?',
                onOk: () => this.onDeleteCallback(requirement.id)
            });
        },
        onDeleteCallback(id) {
            axios({
                method: 'POST',
                type: 'JSON',
                url: '/requirement/delete',
                data: { id: id }
            }).then(response => {
                this.isDeleteLoading = false;
                if (response.data.status == 1) {
                    this.$root.prompt(response.data.text);
                    this.getRequirements();
                } else this.$root.prompt(response.data.text);
            }).catch(error => {
                this.isDeleteLoading = false;
                this.$root.prompt(error.response.data.message);
            });
        },
        editRequirement(requirement) {
            this.requirement = requirement;
            this.isRequirementVisible = true;
        },
        saveRequirement() {
            this.isSaveLoading = true;

            axios({
                method: 'POST',
                type: 'JSON',
                url: '/requirement/save',
                data: this.requirement
            }).then(response => {
                this.isSaveLoading = false;
                if (response.data.status == 1) {
                    this.$root.prompt(response.data.text);
                    this.requirement = {};
                    this.isRequirementVisible = false;
                    this.getRequirements();
                } else this.$root.prompt(response.data.text);
            }).catch(error => {
                this.isSaveLoading = false;
                this.$root.prompt(error.response.data.message);
            });
        },
        getRequirements() {
            this.isLoading = true;

            axios({
                method: 'GET',
                type: 'JSON',
                url: '/requirements'
            }).then(response => {
                this.isLoading = false;
                if (response.data.status == 1) {
                    this.requirements = response.data.result;
                } else this.$root.prompt(response.data.text);
            }).catch(error => {
                this.isLoading = false;
                this.$root.prompt(error.response.data.message);
            });
        },
        formatDate
    }
}
</script>
