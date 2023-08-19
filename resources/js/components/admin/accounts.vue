<template>
    <div class="flex content-between">
        <p>Accounts</p>
        <va-button @click="isAccountVisible = true">Create</va-button>
    </div>
    <div class="va-table-responsive">
        <table class="va-table">
            <thead>
                <tr>
                    <th>Email</th>
                    <th>Type</th>
                    <th>Created at</th>
                    <th>Status</th>
                    <th>Action</th>
                </tr>
            </thead>
            <tbody>
                <tr
                v-for="acc in accounts"
                :key="acc.id"
                >
                    <td>{{ acc.email }}</td>
                    <td>{{ getUserType(acc.user_type) }}</td>
                    <td>{{ formatDate(acc.created_at, 'MMM. DD, YYYY', 'N/A') }}</td>
                    <td>
                        <va-badge
                        :text="acc.deleted_at ? 'Inactive' : 'Active'"
                        :color="acc.deleted_at ? 'warning' : 'success'"
                        />
                    </td>
                    <td>
                        <va-button @click="editAccount(acc)" :disabled="acc.id == sessionId ? true : false">Edit</va-button>&nbsp;
                        <va-button @click="deleteAccount(acc)" :disabled="acc.id == sessionId ? true : false">Delete</va-button>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    <va-modal
    v-model="isAccountVisible"
    hide-default-actions
    >
        <h4 class="va-h4">
            Edit Account
        </h4>
        <div>
            <va-input
                v-model="account.email"
                type="email"
                label="E-mail Address"
                class="mb-2 w-full"
                :rules="[(v) => v && v.length > 0 || 'E-mail address is empty']"
                outline
            />
            <va-select
                v-model="account.user_type"
                class="mb-2 w-full"
                :options="accountTypes"
                outline
                text-by="text"
                value-by="value"
            />
        </div>
        <template #footer>
            <va-button color="#FFFFFF" @click="isAccountVisible = false, account = {}">Cancel</va-button>&nbsp;
            <va-button @click="saveAccount">Save</va-button>
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
            isEditLoading: false,
            isAccountVisible: false,
            accountTypes: [
                { value: 0, text: "Student" },
                { value: 1, text: "Instructor" },
                { value: 2, text: "Secretary" },
                { value: 3, text: "Super Admin" }
            ],
            accounts: [],
            account: {}
        };
    },
    props: ['sessionId'],
    mounted() {
        this.getAccounts();
    },
    methods: {
        deleteAccount(account) {
            this.$vaModal.init({
                message: 'Are you sure you want to delete ' + account.email + '?',
                onOk: () => this.onDeleteCallback(account.id)
            });
        },
        onDeleteCallback(id) {
            axios({
                method: 'POST',
                type: 'JSON',
                url: '/account/delete',
                data: { id: id }
            }).then(response => {
                this.isDeleteLoading = false;
                if (response.data.status == 1) {
                    this.$root.prompt(response.data.text);
                    this.getAccounts();
                } else this.$root.prompt(response.data.text);
            }).catch(error => {
                this.isDeleteLoading = false;
                this.$root.prompt(error.response.data.message);
            });
        },
        editAccount(account) {
            this.account = account;
            this.isAccountVisible = true;
        },
        saveAccount() {
            this.isEditLoading = true;

            axios({
                method: 'POST',
                type: 'JSON',
                url: '/account/save',
                data: this.account
            }).then(response => {
                this.isEditLoading = false;
                if (response.data.status == 1) {
                    this.$root.prompt(response.data.text);
                    this.account = {};
                    this.isAccountVisible = false;
                    this.getAccounts();
                } else this.$root.prompt(response.data.text);
            }).catch(error => {
                this.isEditLoading = false;
                this.$root.prompt(error.response.data.message);
            });
        },
        getAccounts() {
            this.isLoading = true;

            axios({
                method: 'GET',
                type: 'JSON',
                url: '/get_accounts'
            }).then(response => {
                this.isLoading = false;
                if (response.data.status == 1) {
                    this.accounts = response.data.result;
                } else this.$root.prompt(response.data.text);
            }).catch(error => {
                this.isLoading = false;
                this.$root.prompt(error.response.data.message);
            });
        },
        getUserType(type) {
            if (type == 0) return "Student";
            else if (type == 1) return "Instructor";
            else if (type == 2) return "Secretary";
            else if (type == 3) return "Super Admin";
            else return "N/A";
        },
        formatDate
    }
}
</script>
