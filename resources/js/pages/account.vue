<template>
    <div class="h-screen">
        <navigation />
        <div class="flex items-start min-h-[calc(100vh-62px)]">
            <div class="flex items-center w-full py-8 min-h-[calc(100vh-62px)]" style="position: sticky; top:62px;">
                <div class="shrink m-auto">
                    <h5 class="text-base">
                        Account Details
                    </h5>
                    <va-button
                    preset="primary"
                    class="mr-6 mb-2"
                    icon="logout"
                    @click="$root.redirectToPage('/logout')"
                    >
                        Logout
                    </va-button>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import navmenu from '@/components/navbar.vue';

export default {
    components: {
        navigation: navmenu
    }
}
</script>
