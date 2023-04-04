## Import VSCode extensions

### Update extensions list

`code --list-extensions > extensions.list`

### Import extensions

At the new machine run:

`cat extensions.list |% { code --install-extension $_}`