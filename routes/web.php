<?php

use App\Http\Controllers\AccountController;
use App\Http\Controllers\HelpController;
use App\Http\Controllers\NewsController;
use App\Http\Controllers\PageController;
use App\Http\Controllers\RequirementController;
use App\Http\Controllers\RoomController;
use App\Http\Controllers\VehicleController;
use Illuminate\Support\Facades\Route;

/*
|--------------------------------------------------------------------------
| Web Routes
|--------------------------------------------------------------------------
|
| Here is where you can register web routes for your application. These
| routes are loaded by the RouteServiceProvider and all of them will
| be assigned to the "web" middleware group. Make something great!
|
*/

Route::get('/', [PageController::class, 'index']);
Route::get('/dashboard', [PageController::class, 'dashboard']);
Route::get('/account', [PageController::class, 'account']);
Route::get('/accounts', [PageController::class, 'account_management']);
Route::get('/faqs', [HelpController::class, 'get']);
Route::get('/get_accounts', [AccountController::class, 'get_accounts']);
Route::get('/logout', [AccountController::class, 'logout']);
Route::get('/news/get', [NewsController::class, 'get']);
Route::get('/news', [PageController::class, 'get']);
Route::get('/requirements', [RequirementController::class, 'get']);
Route::get('/rooms', [RoomController::class, 'get']);
Route::get('/settings', [PageController::class, 'settings']);
Route::get('/vehicles', [VehicleController::class, 'get']);

Route::prefix('account')->group(function () {
    Route::post('delete', [AccountController::class, 'delete_account']);
    Route::post('save', [AccountController::class, 'save_account']);
});
Route::prefix('faq')->group(function () {
    Route::post('delete', [HelpController::class, 'delete']);
    Route::post('save', [HelpController::class, 'save']);
});
Route::prefix('news')->group(function () {
    Route::post('delete', [NewsController::class, 'delete']);
    Route::post('save', [NewsController::class, 'save']);
});
Route::prefix('requirement')->group(function () {
    Route::post('delete', [RequirementController::class, 'delete']);
    Route::post('save', [RequirementController::class, 'save']);
});
Route::prefix('room')->group(function () {
    Route::post('delete', [RoomController::class, 'delete']);
    Route::post('save', [RoomController::class, 'save']);
});
Route::prefix('vehicle')->group(function () {
    Route::post('delete', [VehicleController::class, 'delete']);
    Route::post('save', [VehicleController::class, 'save']);
});

Route::post('/login', [AccountController::class, 'login']);
