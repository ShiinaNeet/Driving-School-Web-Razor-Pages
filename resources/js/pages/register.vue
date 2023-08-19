<template>

</template>

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
