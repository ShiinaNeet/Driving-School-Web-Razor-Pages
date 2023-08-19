<?php

namespace App\Http\Controllers;

use App\Libraries\SharedFunctions;
use App\Models\Vehicle;
use Illuminate\Http\Request;

class VehicleController extends Controller
{
    public function delete(Request $request)
    {
        $rs = SharedFunctions::default_msg();
        $query = Vehicle::find($request->id);
        if ($query->delete()) $rs = SharedFunctions::success_msg("Vehicle deleted successfully!");
        return response()->json($rs);
    }

    public function get()
    {
        $query = Vehicle::orderBy('created_at', 'DESC')->get();
        $rs = SharedFunctions::success_msg('Success');
        $rs['result'] = $query;
        return response()->json($rs);
    }

    public function save(Request $request)
    {
        $rs = SharedFunctions::default_msg();
        $this->validate($request, [
            'name' => 'required|max:255',
            'description' => 'max:255',
            'capacity' => 'required',
            'transmission' => 'required'
        ]);
        if (isset($request->id)) $vehicle = Vehicle::find($request->id);
        else $vehicle = new Vehicle();
        $vehicle->name = $request->name;
        $vehicle->description = $request->description;
        $vehicle->capacity = $request->capacity;
        $vehicle->transmission = $request->transmission;
        if ($vehicle->save()) $rs = SharedFunctions::success_msg("Vehicle saved successfully!");
        return response()->json($rs);
    }
}
