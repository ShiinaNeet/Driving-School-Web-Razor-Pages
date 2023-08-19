<?php

namespace App\Libraries;

class SharedFunctions
{
    public static function default_msg()
    {
        return [
            'status'    => 0,
            'title'     => "Oops!",
            'text'      => "Something went wrong!",
            'type'      => 'error'
        ];
    }

    public static function success_msg($message = "Successfully!")
    {
        return [
            'status'    => 1,
            'title'     => "Success!",
            'text'      => $message,
            'type'      => 'success'
        ];
    }

    public static function prompt_msg($message = "Invalid!")
    {
        return[
            'status' => 0,
            'title' => 'Oops!',
            'text' => $message,
            'type' => 'error'
        ];
    }

    public static function query_log($builder)
    {
        $query = str_replace(array('?'), array('\'%s\''), $builder->toSql());
        $query = vsprintf($query, $builder->getBindings());
        dd($query);
    }
}
