Metamesh
========

![gif](https://i.imgur.com/4ldi2dH.gif)
![gif](https://i.imgur.com/GXz7Q41.gif)

**Metamesh** is a Unity package that generates primitive meshes. It works as a
custom asset importer but doesn't import anything from an input file. Instead
of importing a file, it reads properties from a metafile and procedurally
generates a mesh.

At the moment, it only supports very basic primitive shapes.

- Plane
- Box
- Sphere (UV sphere)
- Icosphere
- Cylinder
- Rounded box
- Disc
- Ring

Related Project
---------------

- [Metatex] - Metadata-only texture asset importer

[Metatex]: https://github.com/keijiro/Metatex

How To Install
--------------

This package uses the [scoped registry] feature to resolve package
dependencies. Open the Package Manager page in the Project Settings window and
add the following entry to the Scoped Registries list:

- Name: `Keijiro`
- URL: `https://registry.npmjs.com`
- Scope: `jp.keijiro`

![Scoped Registry](https://user-images.githubusercontent.com/343936/162576797-ae39ee00-cb40-4312-aacd-3247077e7fa1.png)

Now you can install the package from My Registries page in the Package Manager
window.

![My Registries](https://user-images.githubusercontent.com/343936/162576825-4a9a443d-62f9-48d3-8a82-a3e80b486f04.png)

[scoped registry]: https://docs.unity3d.com/Manual/upm-scoped.html
