<template>
    <div class="flex content-between">
        <p>Help</p>
        <va-button @click="isHelpVisible = true">Create</va-button>
    </div>
    <div class="va-table-responsive">
        <table class="va-table">
            <thead>
                <tr>
                    <th>Question</th>
                    <th>Answer</th>
                    <th>Created at</th>
                    <th>Action</th>
                </tr>
            </thead>
            <tbody>
                <tr
                v-for="h in helpArr"
                :key="h.id"
                >
                    <td>{{ h.question }}</td>
                    <td>{{ h.answer }}</td>
                    <td>{{ formatDate(h.created_at, 'MMM. DD, YYYY', 'N/A') }}</td>
                    <td>
                        <va-button @click="editHelp(h)">Edit</va-button>&nbsp;
                        <va-button @click="deleteHelp(h)">Delete</va-button>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    <va-modal
    v-model="isHelpVisible"
    hide-default-actions
    >
        <h4 class="va-h4">
            {{ help.id ? 'Edit' : 'Create' }} Help
        </h4>
        <div>
            <va-input
                v-model="help.question"
                type="text"
                label="Question"
                class="mb-2 w-full"
                outline
            />
            <va-input
                v-model="help.answer"
                type="text"
                label="Answer"
                class="mb-2 w-full"
                outline
            />
        </div>
        <template #footer>
            <va-button color="#FFFFFF" @click="isHelpVisible = false, help = {}">Cancel</va-button>&nbsp;
            <va-button @click="saveHelp">Save</va-button>
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
            isHelpVisible: false,
            helpArr: [],
            help: {}
        };
    },
    mounted() {
        this.getHelp();
    },
    methods: {
        deleteHelp(help) {
            this.$vaModal.init({
                message: 'Are you sure you want to delete?',
                onOk: () => this.onDeleteCallback(help.id)
            });
        },
        onDeleteCallback(id) {
            axios({
                method: 'POST',
                type: 'JSON',
                url: '/faq/delete',
                data: { id: id }
            }).then(response => {
                this.isDeleteLoading = false;
                if (response.data.status == 1) {
                    this.$root.prompt(response.data.text);
                    this.getHelp();
                } else this.$root.prompt(response.data.text);
            }).catch(error => {
                this.isDeleteLoading = false;
                this.$root.prompt(error.response.data.message);
            });
        },
        editHelp(help) {
            this.help = help;
            this.isHelpVisible = true;
        },
        saveHelp() {
            this.isSaveLoading = true;

            axios({
                method: 'POST',
                type: 'JSON',
                url: '/faq/save',
                data: this.help
            }).then(response => {
                this.isSaveLoading = false;
                if (response.data.status == 1) {
                    this.$root.prompt(response.data.text);
                    this.help = {};
                    this.isHelpVisible = false;
                    this.getHelp();
                } else this.$root.prompt(response.data.text);
            }).catch(error => {
                this.isSaveLoading = false;
                this.$root.prompt(error.response.data.message);
            });
        },
        getHelp() {
            this.isLoading = true;

            axios({
                method: 'GET',
                type: 'JSON',
                url: '/faqs'
            }).then(response => {
                this.isLoading = false;
                if (response.data.status == 1) {
                    this.helpArr = response.data.result;
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
