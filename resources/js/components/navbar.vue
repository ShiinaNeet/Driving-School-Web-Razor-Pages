<template>
    <va-navbar
    color="primary"
    style="position: sticky; top:0px; z-index: 1; padding-top: 11px; padding-bottom: 11px; height: 62px;"
    >
        <template #left>
            <va-navbar-item class="logo self-center">
                LOGO
            </va-navbar-item>
        </template>
        <template #right>
            <va-navbar-item class="self-center">
                <div class="px-3 py-2 rounded text-blue-600 bg-slate-100 hover:text-white hover:bg-blue-600">
                    <va-icon
                    />
                    <span class="align-middle select-none">Enroll</span>
                </div>
            </va-navbar-item>
            <va-navbar-item
            class="self-center"
            @click="path != '/dashboard' ? $root.redirectToPage('/dashboard') : ''"
            :class="path == '/dashboard' ? 'rounded bg-blue-600 active' : ''"
            >
                <div class="px-3 py-2 rounded hover:bg-blue-600">
                    <va-icon
                    />
                    <span class="align-middle select-none">Home</span>
                </div>
            </va-navbar-item>
            <va-navbar-item class="self-center">
                <div class="px-3 py-2 rounded hover:bg-blue-600">
                    <va-icon
                    />
                    <span class="align-middle select-none">News</span>
                </div>
            </va-navbar-item>
            <va-navbar-item class="self-center">
                <div class="px-2.5 py-2 rounded hover:bg-blue-600" title="Help">
                    <va-icon
                    name="help"
                    />
                </div>
            </va-navbar-item>
            <va-navbar-item class="self-center">
                <div class="px-2.5 py-2 rounded hover:bg-blue-600" title="Notifications">
                    <va-badge
                    overlap
                    dot
                    color="#EF4444"
                    :offset="[-2, 2]"
                    style="--va-badge-text-wrapper-border-radius: 40px;"
                    >
                        <va-icon
                        name="notifications"
                        />
                    </va-badge>
                </div>
            </va-navbar-item>
            <va-navbar-item class="self-center">
                <div class="px-2.5 py-2 rounded hover:bg-blue-600" title="Cart">
                    <va-icon
                    name="shopping_cart"
                    />
                </div>
            </va-navbar-item>
            <!-- <va-navbar-item class="self-center" @click="$root.redirectToPage('/account')"> -->
            <!-- <va-navbar-item class="self-center" @click="logout"> -->
                <!-- <div class="px-2.5 py-2 rounded hover:bg-blue-600 whitespace-nowrap">
                    <va-icon
                    name="account_circle"
                    />
                    <span class="ml-2 align-middle select-none">{{ accountDetails.fname }} {{ accountDetails.lname.substring(0, 1).toUpperCase() + "." }}</span>
                </div>
            </va-navbar-item> -->
            <va-navbar-item class="self-center">
                <va-dropdown
                class="align-middle"
                stickToEdges
                >
                    <template #anchor>
                        <div class="px-2.5 py-2 rounded hover:bg-blue-600 whitespace-nowrap">
                            <va-icon
                            name="account_circle"
                            />
                            <span class="ml-2 align-middle select-none">{{ accountDetails.fname }} {{ accountDetails.lname.substring(0, 1).toUpperCase() + "." }}</span>
                        </div>
                    </template>

                    <va-dropdown-content>
                        <div
                        class="text-neutral-500 p-0.5 select-none hover:text-blue-600"
                        @click="$root.redirectToPage('/account')"
                        :class="text-blue-600"
                        >
                            <p>
                                <va-icon
                                class="mr-1 mb-1"
                                name="person_outline"
                                size="small"
                                />
                                Profile
                            </p>
                        </div>
                        <va-divider />
                        <div class="text-neutral-500 p-0.5 select-none hover:text-blue-600" @click="$root.redirectToPage('/settings')">
                            <p>
                                <va-icon
                                class="mr-1 mb-1"
                                name="settings"
                                size="small"
                                />
                                Settings
                            </p>
                        </div>
                        <va-divider />
                        <div class="text-neutral-500 p-0.5 select-none hover:text-blue-600" @click="$root.redirectToPage('/logout')">
                            <p>
                                <va-icon
                                class="mr-1 mb-1"
                                name="logout"
                                size="small"
                                />
                                Logout
                            </p>
                        </div>
                    </va-dropdown-content>
                </va-dropdown>
            </va-navbar-item>
        </template>
    </va-navbar>
</template>

<style scoped>
.va-navbar__right>.va-navbar__item {
    margin-right: 0.3em;
}
.va-navbar__right>.va-navbar__item .active {
    pointer-events: none;
}
</style>

<script>
export default {
    data () {
        return {
            accountDetails: {
                fname: 'Juan',
                lname: 'Cruz'
            },
            path: window.location.pathname
        };
    }
}
</script>
