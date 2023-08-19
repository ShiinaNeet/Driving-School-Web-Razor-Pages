<?php

namespace Database\Seeders;

use App\Models\User;
use Illuminate\Database\Seeder;

class UserSeeder extends Seeder
{
    /**
     * Run the database seeds.
     */
    public function run(): void
    {
        User::truncate();
        User::create([
            'user_type' => User::TYPE_ADMIN,
            'email'     => 'admin@email.com',
            'password'  => bcrypt('password')
        ]);
    }
}
