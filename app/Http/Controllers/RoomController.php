<?php

namespace App\Http\Controllers;

use App\Libraries\SharedFunctions;
use App\Models\Room;
use Illuminate\Http\Request;

class RoomController extends Controller
{
    public function delete(Request $request)
    {
        $rs = SharedFunctions::default_msg();
        $query = Room::find($request->id);
        if ($query->delete()) $rs = SharedFunctions::success_msg("Room deleted successfully!");
        return response()->json($rs);
    }

    public function get()
    {
        $query = Room::orderBy('created_at', 'DESC')->get();
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
        if (isset($request->id)) $room = Room::find($request->id);
        else $room = new Room();
        $room->name = $request->name;
        $room->description = $request->description;
        if ($room->save()) $rs = SharedFunctions::success_msg("Room saved successfully!");
        return response()->json($rs);
    }
}
