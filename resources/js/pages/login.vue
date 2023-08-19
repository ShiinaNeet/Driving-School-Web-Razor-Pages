<template>
    <div class="h-screen">
        <va-navbar
        color="primary"
        style="position: sticky; top:0px; z-index: 1; padding-top: 19px; padding-bottom: 19px; height: 62px;"
        >
            <template #left>
                <va-navbar-item class="logo">
                    LOGO
                </va-navbar-item>
            </template>
        </va-navbar>
        <div class="flex items-start min-h-[calc(100vh-62px)]">
            <div class="flex items-center w-9/12 py-8 min-h-[calc(100vh-62px)]" style="position: sticky; top:62px;">
                <div class="shrink m-auto w-[min(400px,75vw)]" id="login-form-wrapper">
                    <h5 class="va-h5">
                        Welcome, Wheels User
                    </h5>
                    <h5 class="va-text-secondary">
                        Please sign in to continue
                    </h5>
                    <div class="mt-10">
                        <va-input
                            v-model="email"
                            type="email"
                            label="E-mail Address"
                            class="mb-2 w-full"
                            :rules="[(v) => v && v.length > 0 || 'E-mail address is empty']"
                            :error="isInvalidLogin"
                            outline
                        />
                        <va-input
                            v-model="password"
                            :type="isPasswordVisible ? 'text' : 'password'"
                            label="Password"
                            class="mb-3 w-full"
                            :rules="[(v) => v && v.length > 0 || 'Password is empty']"
                            :error="isInvalidLogin"
                            :error-messages="invalidLoginMessage"
                            outline
                        >
                            <template #appendInner>
                                <va-icon
                                    :name="isPasswordVisible ? 'visibility_off' : 'visibility'"
                                    size="small"
                                    color="--va-primary"
                                    @click="isPasswordVisible = !isPasswordVisible"
                                />
                            </template>
                        </va-input>
                    </div>
                    <div class="mt-6 mb-6">
                        <va-button
                            @click="login"
                            :loading="isLoading"
                            block
                        >
                            Log in
                        </va-button>
                    </div>
                    <div class="flex justify-between">
                        <a href="#" class="va-link hover:underline">Forgot password?</a>
                        <span class="text-right">
                            No account?
                            <a href="#" class="va-link hover:underline">Sign up now</a>
                        </span>
                    </div>
                    <div class="my-8 text-gray-400">
                        <va-divider orientation="center">
                            <span class="px-2">OR</span>
                        </va-divider>
                    </div>
                    <div class="mt-6">
                        <va-button
                            preset="secondary"
                            border-color="primary"
                            icon="facebook"
                            block
                            disabled
                        >
                            Log in with Facebook
                        </va-button>
                    </div>
                </div>
            </div>
            <div class="flex items-center w-1/4 pt-8 min-h-[calc(100vh-62px)]">
                <div class="block text-center mx-3">
                    <h5 class="text-base">
                        Check out our on-going promotions
                    </h5>
                    <div class="mt-4">
                        <div>
                            <VaSkeleton tag="h6" variant="text" class="va-h6" />
                            <VaSkeleton class="mb-3" />
                            <VaSkeleton variant="text" :lines="3" />
                        </div>
                        <div>
                            <VaSkeleton tag="h6" variant="text" class="va-h6" />
                            <VaSkeleton height="15rem" class="mb-3" />
                            <VaSkeleton variant="text" :lines="5" />
                        </div>
                        <div>
                            <VaSkeleton tag="h6" variant="text" class="va-h6" />
                            <VaSkeleton height="7rem" class="mb-3" />
                            <VaSkeleton variant="text" :lines="2" />
                        </div>
                        <div>
                            <VaSkeleton tag="h6" variant="text" class="va-h6" />
                            <VaSkeleton height="7rem" class="mb-3" />
                            <VaSkeleton variant="text" :lines="2" />
                        </div>
                        <div>
                            <VaSkeleton tag="h6" variant="text" class="va-h6" />
                            <VaSkeleton class="mb-3" />
                            <VaSkeleton variant="text" :lines="3" />
                        </div>
                        <div>
                            <VaSkeleton tag="h6" variant="text" class="va-h6" />
                            <VaSkeleton class="mb-3" />
                            <VaSkeleton variant="text" :lines="3" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<style scoped>
#login-form-wrapper::after {
    content: "";
    position: absolute;
    top: 30px;
    bottom: 30px;
    right: 0px;
    border-right: 1px solid #DEE5F2;
}
</style>

<script>
export default {
    data () {
        return {
            isPasswordVisible: false,
            isInvalidLogin: false,
            invalidLoginMessage: "Please check your e-mail/password and try again",
            isLoading: false,
            email: null,
            password: null,
            delay: 500
        };
    },
    methods: {
        login() {
            this.isLoading = true;
            this.isInvalidLogin = false;

            axios({
                method: 'POST',
                type: 'JSON',
                url: '/login',
                data: {
                    email: this.email,
                    password: this.password
                }
            }).then(response => {
                this.isLoading = false;

                if (response.data.status == 1) {
                    setTimeout(() => {
                        window.location = response.data.redirect;
                    }, this.delay);
                } else {
                    this.isInvalidLogin = true;
                    // this.$root.prompt(response.data.text);
                }
            }).catch(error => {
                this.isLoading = false;
                this.isInvalidLogin = true;

                // this.$root.prompt(error.response.data.message);
            });
        }
    }
}
</script>
