<template>
    <div class="flex content-between">
        <p>Rooms</p>
        <va-button @click="isRoomVisible = true">Create</va-button>
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
                v-for="room in rooms"
                :key="room.id"
                >
                    <td>{{ room.name }}</td>
                    <td>{{ room.description }}</td>
                    <td>{{ formatDate(room.created_at, 'MMM. DD, YYYY', 'N/A') }}</td>
                    <td>
                        <va-button @click="editRoom(room)">Edit</va-button>&nbsp;
                        <va-button @click="deleteRoom(room)">Delete</va-button>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    <va-modal
    v-model="isRoomVisible"
    hide-default-actions
    >
        <h4 class="va-h4">
            {{ room.id ? 'Edit' : 'Create' }} Room
        </h4>
        <div>
            <va-input
                v-model="room.name"
                type="text"
                label="Name"
                class="mb-2 w-full"
                outline
            />
            <va-input
                v-model="room.description"
                type="text"
                label="Description"
                class="mb-2 w-full"
                outline
            />
        </div>
        <template #footer>
            <va-button color="#FFFFFF" @click="isRoomVisible = false, room = {}">Cancel</va-button>&nbsp;
            <va-button @click="saveRoom">Save</va-button>
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
            isRoomVisible: false,
            rooms: [],
            room: {}
        };
    },
    mounted() {
        this.getRooms();
    },
    methods: {
        deleteRoom(room) {
            this.$vaModal.init({
                message: 'Are you sure you want to delete ' + room.name + '?',
                onOk: () => this.onDeleteCallback(room.id)
            });
        },
        onDeleteCallback(id) {
            axios({
                method: 'POST',
                type: 'JSON',
                url: '/room/delete',
                data: { id: id }
            }).then(response => {
                this.isDeleteLoading = false;
                if (response.data.status == 1) {
                    this.$root.prompt(response.data.text);
                    this.getRooms();
                } else this.$root.prompt(response.data.text);
            }).catch(error => {
                this.isDeleteLoading = false;
                this.$root.prompt(error.response.data.message);
            });
        },
        editRoom(room) {
            this.room = room;
            this.isRoomVisible = true;
        },
        saveRoom() {
            this.isSaveLoading = true;

            axios({
                method: 'POST',
                type: 'JSON',
                url: '/room/save',
                data: this.room
            }).then(response => {
                this.isSaveLoading = false;
                if (response.data.status == 1) {
                    this.$root.prompt(response.data.text);
                    this.room = {};
                    this.isRoomVisible = false;
                    this.getRooms();
                } else this.$root.prompt(response.data.text);
            }).catch(error => {
                this.isSaveLoading = false;
                this.$root.prompt(error.response.data.message);
            });
        },
        getRooms() {
            this.isLoading = true;

            axios({
                method: 'GET',
                type: 'JSON',
                url: '/rooms'
            }).then(response => {
                this.isLoading = false;
                if (response.data.status == 1) {
                    this.rooms = response.data.result;
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
