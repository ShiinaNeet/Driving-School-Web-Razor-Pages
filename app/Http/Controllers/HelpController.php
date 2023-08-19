<?php

namespace App\Http\Controllers;

use App\Libraries\SharedFunctions;
use App\Models\Help;
use Illuminate\Http\Request;

class HelpController extends Controller
{
    public function delete(Request $request)
    {
        $rs = SharedFunctions::default_msg();
        $query = Help::find($request->id);
        if ($query->delete()) $rs = SharedFunctions::success_msg("FAQ deleted successfully!");
        return response()->json($rs);
    }

    public function get()
    {
        $query = Help::orderBy('created_at', 'DESC')->get();
        $rs = SharedFunctions::success_msg('Success');
        $rs['result'] = $query;
        return response()->json($rs);
    }

    public function save(Request $request)
    {
        $rs = SharedFunctions::default_msg();
        $this->validate($request, [
            'question' => 'required|max:255',
            'answer' => 'required|max:1500'
        ]);
        if (isset($request->id)) $help = Help::find($request->id);
        else $help = new Help();
        $help->question = $request->question;
        $help->answer = $request->answer;
        if ($help->save()) $rs = SharedFunctions::success_msg("FAQ saved successfully!");
        return response()->json($rs);
    }
}
