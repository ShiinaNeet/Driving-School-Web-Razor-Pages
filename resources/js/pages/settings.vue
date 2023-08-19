<template>
    <div class="h-screen">
        <navigation />

        <va-tabs
        v-model="value"
        >
            <template #tabs>
                <va-tab
                v-for="tab in tabs"
                :key="tab.title"
                :name="tab.title"
                >
                    <va-icon
                        :name="tab.icon"
                        size="small"
                        class="mr-2"
                    />
                    {{ tab.title }}
                </va-tab>
            </template>
            <template v-if="value == 'Accounts'">
                <accounts :session-id="sessionId" />
            </template>
            <template v-else-if="value == 'Help'">
                <help />
            </template>
            <template v-else-if="value == 'News'">
                <news />
            </template>
            <template v-else-if="value == 'Requirements'">
                <requirements />
            </template>
            <template v-else-if="value == 'Rooms'">
                <rooms />
            </template>
            <template v-else-if="value == 'Vehicles'">
                <vehicles />
            </template>
        </va-tabs>
    </div>
</template>

<script>
import navmenu from '@/components/navbar.vue';
import accmenu from '@/components/admin/accounts.vue';
import helpmenu from '@/components/admin/help.vue';
import newsmenu from '@/components/admin/news.vue';
import requirementmenu from '@/components/admin/requirements.vue';
import roomsmenu from '@/components/admin/rooms.vue';
import vehiclesmenu from '@/components/admin/vehicles.vue';

const TABS = [
  { icon: "people", title: "Accounts" },
  { icon: "feed", title: "Help" },
  { icon: "feed", title: "News" },
  { icon: "settings", title: "Vehicles" },
  { icon: "settings", title: "Rooms" },
  { icon: "settings", title: "Requirements" }
];

export default {
    components: {
        navigation: navmenu,
        accounts: accmenu,
        help: helpmenu,
        news: newsmenu,
        requirements: requirementmenu,
        rooms: roomsmenu,
        vehicles: vehiclesmenu
    },
    props: ['sessionId'],
    data () {
        return {
            tabs: TABS,
            value: TABS[0].title
        };
    },
    computed: {
        currentTab() {
            return this.tabs.find(({ title }) => title === this.value);
        },
    }
}
</script>
