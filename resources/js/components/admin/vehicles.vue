<template>
    <div class="flex content-between">
        <p>Vehicles</p>
        <va-button @click="isVehicleVisible = true">Create</va-button>
    </div>
    <div class="va-table-responsive">
        <table class="va-table">
            <thead>
                <tr>
                    <th>Name</th>
                    <th>Description</th>
                    <th>Capacity</th>
                    <th>Transmission</th>
                    <th>Created at</th>
                    <th>Action</th>
                </tr>
            </thead>
            <tbody>
                <tr
                v-for="vehicle in vehicles"
                :key="vehicle.id"
                >
                    <td>{{ vehicle.name }}</td>
                    <td>{{ vehicle.description }}</td>
                    <td>{{ vehicle.capacity }}</td>
                    <td>{{ vehicle.transmission == 0 ? "Automatic" : "Manual" }}</td>
                    <td>{{ formatDate(vehicle.created_at, 'MMM. DD, YYYY', 'N/A') }}</td>
                    <td>
                        <va-button @click="editVehicle(vehicle)">Edit</va-button>&nbsp;
                        <va-button @click="deleteVehicle(vehicle)">Delete</va-button>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    <va-modal
    v-model="isVehicleVisible"
    hide-default-actions
    >
        <h4 class="va-h4">
            {{ vehicle.id ? 'Edit' : 'Create' }} Vehicle
        </h4>
        <div>
            <va-input
                v-model="vehicle.name"
                type="text"
                label="Name"
                class="mb-2 w-full"
                outline
            />
            <va-input
                v-model="vehicle.description"
                type="text"
                label="Description"
                class="mb-2 w-full"
                outline
            />
            <va-counter
                v-model="vehicle.capacity"
                label="Capacity"
                class="mb-2 w-full"
                :min="1"
                outline
            />
            <va-select
                v-model="vehicle.transmission"
                label="Transmission"
                class="mb-2 w-full"
                :options="transmissions"
                outline
                text-by="text"
                value-by="value"
            />
        </div>
        <template #footer>
            <va-button color="#FFFFFF" @click="isVehicleVisible = false, vehicle = { capacity: 1 }">Cancel</va-button>&nbsp;
            <va-button @click="saveVehicle">Save</va-button>
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
            isVehicleVisible: false,
            transmissions: [
                { value: 0, text: "Automatic" },
                { value: 1, text: "Manual" }
            ],
            vehicles: [],
            vehicle: { capacity: 1 }
        };
    },
    mounted() {
        this.getVehicles();
    },
    methods: {
        deleteVehicle(vehicle) {
            this.$vaModal.init({
                message: 'Are you sure you want to delete ' + vehicle.name + '?',
                onOk: () => this.onDeleteCallback(vehicle.id)
            });
        },
        onDeleteCallback(id) {
            axios({
                method: 'POST',
                type: 'JSON',
                url: '/vehicle/delete',
                data: { id: id }
            }).then(response => {
                this.isDeleteLoading = false;
                if (response.data.status == 1) {
                    this.$root.prompt(response.data.text);
                    this.getVehicles();
                } else this.$root.prompt(response.data.text);
            }).catch(error => {
                this.isDeleteLoading = false;
                this.$root.prompt(error.response.data.message);
            });
        },
        editVehicle(vehicle) {
            this.vehicle = vehicle;
            this.isVehicleVisible = true;
        },
        saveVehicle() {
            this.isSaveLoading = true;

            axios({
                method: 'POST',
                type: 'JSON',
                url: '/vehicle/save',
                data: this.vehicle
            }).then(response => {
                this.isSaveLoading = false;
                if (response.data.status == 1) {
                    this.$root.prompt(response.data.text);
                    this.vehicle = { capacity: 1 };
                    this.isVehicleVisible = false;
                    this.getVehicles();
                } else this.$root.prompt(response.data.text);
            }).catch(error => {
                this.isSaveLoading = false;
                this.$root.prompt(error.response.data.message);
            });
        },
        getVehicles() {
            this.isLoading = true;

            axios({
                method: 'GET',
                type: 'JSON',
                url: '/vehicles'
            }).then(response => {
                this.isLoading = false;
                if (response.data.status == 1) {
                    this.vehicles = response.data.result;
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
