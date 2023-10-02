# KV - console Key Value store

> I wrote this long long ago, and forgot about it, then found it again, and now, 10+ years later, still use it every day.
>
> -- 2023

Simple Key Value store, a NO-SQL database, available from the commandline.

Original blog post launching it is here: 


- [kv can remember it for you, wholesale](https://secretgeek.net/kv)

Tiny little console app to demonstrate `IStashy`

More about IStashy here:

- [Stashy is a Really simple Key Value store](https://secretgeek.net/stashy_gist)

IStashy is the simplest possible no-sql database, it need not scale to millions of objects -- but is just right for a lot of projects.


## Usage

	kv.exe help

	kv -- a key-value store integrated with the clipboard.
	inspired by: https://github.com/stevenleeg/boo

	usage:
	kv name fred smith
		saves the value, 'fred smith' under the key, 'name'
	kv name
		retrieves the value 'fred smith' straight to your clipboard.
	kv
		lists all keys
	kv n*
		retrieves the first key that matches the pattern n*
	kv -r name
		will remove the key 'name' (and its value) from your store

## Save and retrieve a value:

Teaching by example, if you run this command:

	kv someKey some other stuff

^^ `kv` will store the value `some other stuff` against the key "someKey"

If you then type, `kv someKey` -- you will get the result:  "some other stuff" displayed on the commandline, *and* copied directly to your clipboard.

The keys and values are stored in a file, not just in memory. So they are "permanent".

## List all of the available keys...

Just:

	kv

Or --

	kv *

## Delete a Key (and its value)

	kv -r myKey

...will remove the key "myKey" (and its value) from your store FOREVER.

There's no backup. There's no undo. Just digital oblivion.

## See also

- [Stashy is a Really simple Key Value store](https://secretgeek.net/stashy_gist)
- [kv can remember it for you, wholesale](https://secretgeek.net/kv)
- [kp: console passwords](https://github.com/secretGeek/kp)
