<template>
    <div class="flex content-between">
        <p>News</p>
        <va-button @click="isNewsVisible = true">Create</va-button>
    </div>
    <div class="va-table-responsive">
        <table class="va-table">
            <thead>
                <tr>
                    <th>Title</th>
                    <th>Description</th>
                    <th>Created at</th>
                    <th>Action</th>
                </tr>
            </thead>
            <tbody>
                <tr
                v-for="newsq in newsList"
                :key="newsq.id"
                >
                    <td>{{ newsq.title }}</td>
                    <td>{{ newsq.description }}</td>
                    <td>{{ formatDate(newsq.created_at, 'MMM. DD, YYYY', 'N/A') }}</td>
                    <td>
                        <va-button @click="editNews(newsq)">Edit</va-button>&nbsp;
                        <va-button @click="deleteNews(newsq)">Delete</va-button>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    <va-modal
    v-model="isNewsVisible"
    hide-default-actions
    >
        <h4 class="va-h4">
            {{ news.id ? 'Edit' : 'Create' }} News
        </h4>
        <div>
            <va-input
                v-model="news.title"
                type="text"
                label="Title"
                class="mb-2 w-full"
                outline
            />
            <va-input
                v-model="news.description"
                type="text"
                label="Description"
                class="mb-2 w-full"
                outline
            />
        </div>
        <template #footer>
            <va-button color="#FFFFFF" @click="isNewsVisible = false, news = {}">Cancel</va-button>&nbsp;
            <va-button @click="saveNews">Save</va-button>
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
            isNewsVisible: false,
            newsList: [],
            news: {}
        };
    },
    mounted() {
        this.getNews();
    },
    methods: {
        deleteNews(news) {
            this.$vaModal.init({
                message: 'Are you sure you want to delete ' + news.title + '?',
                onOk: () => this.onDeleteCallback(news.id)
            });
        },
        onDeleteCallback(id) {
            axios({
                method: 'POST',
                type: 'JSON',
                url: '/news/delete',
                data: { id: id }
            }).then(response => {
                this.isDeleteLoading = false;
                if (response.data.status == 1) {
                    this.$root.prompt(response.data.text);
                    this.getNews();
                } else this.$root.prompt(response.data.text);
            }).catch(error => {
                this.isDeleteLoading = false;
                this.$root.prompt(error.response.data.message);
            });
        },
        editNews(news) {
            this.news = news;
            this.isNewsVisible = true;
        },
        saveNews() {
            this.isSaveLoading = true;

            axios({
                method: 'POST',
                type: 'JSON',
                url: '/news/save',
                data: this.news
            }).then(response => {
                this.isSaveLoading = false;
                if (response.data.status == 1) {
                    this.$root.prompt(response.data.text);
                    this.news = {};
                    this.isNewsVisible = false;
                    this.getNews();
                } else this.$root.prompt(response.data.text);
            }).catch(error => {
                this.isSaveLoading = false;
                this.$root.prompt(error.response.data.message);
            });
        },
        getNews() {
            this.isLoading = true;

            axios({
                method: 'GET',
                type: 'JSON',
                url: '/news'
            }).then(response => {
                this.isLoading = false;
                if (response.data.status == 1) {
                    this.newsList = response.data.result;
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
