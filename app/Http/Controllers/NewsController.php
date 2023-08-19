<?php

namespace App\Http\Controllers;

use App\Libraries\SharedFunctions;
use App\Models\News;
use Illuminate\Http\Request;

class NewsController extends Controller
{
    public function delete(Request $request)
    {
        $rs = SharedFunctions::default_msg();
        $query = News::find($request->id);
        if ($query->delete()) $rs = SharedFunctions::success_msg("News deleted successfully!");
        return response()->json($rs);
    }

    public function get()
    {
        $news = News::orderBy('created_at', 'DESC')->get();
        $rs = SharedFunctions::success_msg('Success');
        $rs['result'] = $news;
        return response()->json($rs);
    }

    public function save(Request $request)
    {
        $rs = SharedFunctions::default_msg();
        $this->validate($request, [
            'title' => 'required|max:255',
            'description' => 'required|max:1500'
        ]);
        if (isset($request->id)) $news = News::find($request->id);
        else $news = new News();
        $news->title = $request->title;
        $news->description = $request->description;
        if ($news->save()) $rs = SharedFunctions::success_msg("News saved successfully!");
        return response()->json($rs);
    }
}
