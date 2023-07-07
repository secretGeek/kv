# KV

Simple Key Value store db, available from the commandline.

Tiny little console app to demonstrate `IStashy`

IStashy is the simplest possible no-sql database, it need not scale to millions of objects -- but is just right for a lot of projects.

I wrote this a decade or so back and still use it as a place to store and retrieve snippets.

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

^^ `kv` will store the value `some other stuff` against the key `someKey`

If you then type, `kv someKey` -- you will get the result:  `some other stuff` displayed on the commandline, *and* copied directly to your clipboard.


The keys and values are stored in a file, not just in memory. So they are "permanent".


## List all of the available keys...

Just:

	kv


Or --

	kv *


## Delete a Key (and its value)


	kv -r myKey

...will remove the key 'myKey' (and its value) from your store FOREVER.

There's no backup. There's no undo. Just digital oblivion.


