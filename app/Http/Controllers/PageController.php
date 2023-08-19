<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\Auth;

class PageController extends Controller
{
    public function index()
    {
        $data['css'] = ['global'];
        return view('wheels.login', $data);
    }

    public function dashboard()
    {
        $data['css'] = ['global'];
        return view('wheels.dashboard', $data);
    }

    public function account()
    {
        $data['css'] = ['global'];
        return view('wheels.account', $data);
    }

    public function settings()
    {
        $data['css'] = ['global'];
        $data['id'] = Auth::id();
        return view('wheels.settings', $data);
    }
}
