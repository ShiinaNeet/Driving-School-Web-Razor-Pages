<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    /**
     * Run the migrations.
     */
    public function up(): void
    {
        Schema::table('users', function (Blueprint $table) {
            $table->string('first_name')->after('user_type')->nullable();
            $table->string('last_name')->after('first_name')->nullable();
            $table->string('phone', 20)->after('email')->nullable();
            $table->string('address')->after('phone')->nullable();
            $table->date('date_of_birth')->after('address')->nullable();
            $table->enum('gender', ['male', 'female', 'other'])->after('date_of_birth')->nullable();
            $table->string('profile_photo')->after('gender')->nullable();
            $table->enum('status', ['active', 'inactive', 'suspended'])->default('active')->after('profile_photo');
        });
    }

    /**
     * Reverse the migrations.
     */
    public function down(): void
    {
        Schema::table('users', function (Blueprint $table) {
            $table->dropColumn([
                'first_name',
                'last_name',
                'phone',
                'address',
                'date_of_birth',
                'gender',
                'profile_photo',
                'status'
            ]);
        });
    }
};
