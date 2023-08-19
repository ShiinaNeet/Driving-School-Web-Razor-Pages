<?php

namespace App\Http\Controllers;

use App\Libraries\SharedFunctions;
use App\Models\Requirement;
use Illuminate\Http\Request;

class RequirementController extends Controller
{
    public function delete(Request $request)
    {
        $rs = SharedFunctions::default_msg();
        $query = Requirement::find($request->id);
        if ($query->delete()) $rs = SharedFunctions::success_msg("Requirement deleted successfully!");
        return response()->json($rs);
    }

    public function get()
    {
        $query = Requirement::orderBy('created_at', 'DESC')->get();
        $rs = SharedFunctions::success_msg('Success');
        $rs['result'] = $query;
        return response()->json($rs);
    }

    public function save(Request $request)
    {
        $rs = SharedFunctions::default_msg();
        $this->validate($request, [
            'name' => 'required|max:255',
            'description' => 'max:255'
        ]);
        if (isset($request->id)) $query = Requirement::find($request->id);
        else $query = new Requirement();
        $query->name = $request->name;
        $query->description = $request->description;
        if ($query->save()) $rs = SharedFunctions::success_msg("Requirement saved successfully!");
        return response()->json($rs);
    }
}
