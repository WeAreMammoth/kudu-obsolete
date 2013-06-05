﻿$(function () {
    var Vfs = {
        getChildren: function (item) {
            return $.get(item.href);
        },

        createFolder: function (folder) {
            return $.ajax({
                url: folder.href + "/",
                method: "PUT"
            });
        },

        addFiles: function (files) {
            return whenArray(
                $.map(files, function (item) {
                    return $.ajax({
                        url: viewModel.selected().href + "/" + item.name,
                        method: "PUT",
                        data: item.contents,
                        processData: false
                    });
                })
            );
        },

        deleteItems: function (item) {
            var childDeferred;
            if (item.mime !== "inode/directory") {
                childDeferred = $.Deferred().resolve();
            } else {
                // Recursively delete the subtree
                childDeferred = Vfs.getChildren(item).pipe(function (children) {
                    return whenArray($.map(children, Vfs.deleteItems));
                });
            }

            return childDeferred.pipe(function () {
                return $.ajax({
                    url: item.href,
                    method: "DELETE",
                    headers: {
                        "If-Match": "*"
                    },
                })
            });
        }
    };

    var node = function (data, parent) {
        this.parent = parent;
        this.name = ko.observable(data.name);
        this.size = ko.observable(data.size ? (Math.ceil(data.size / 1024) + ' KB') : '');
        this.mime = data.mime;
        this.isDirectory = ko.observable(!data.name || (data.mime === "inode/directory"));
        this.href = data.href;
        this.modifiedTime = ((data.mtime && new Date(data.mtime)) || new Date()).toLocaleString() ;
        this.url = ko.observable(this.isDirectory() ? data.href.replace(/\/vfs\//, "/zip/") : data.href);
        this.appRelativePath = ko.computed(function () {
            if (data.href === '/vfs/') {
                return "";
            }
            var path = data.href.replace(/.*\/vfs($|\/(.*)\/)/, "$2").replace(/\//g, "\\");
            path.length--;
            return path;
        }, this);
        this.children = ko.observableArray([]);
        this.editing = ko.observable(data.editing || false);
        this._childrenFetched = false;

        this.fetchChildren = function (force) {
            var that = this;

            if (force || !that._childrenFetched) {
                viewModel.processing(true);

                return Vfs.getChildren(that)
                .done(function (data) {
                    viewModel.processing(false);
                    if (data && data.length) {
                        var children = that.children;
                        children.removeAll();
                        $.each(data, function () {
                            children.push(new node(this, that));
                        });
                    }
                    that._childrenFetched = true;
                }).promise();
            } else {
                return $.Deferred().resolve().promise();
            }
        }
        this.deleteItem = function () {
            if (confirm("Are you sure you want to delete '" + this.name() + "'?")) {
                var that = this;
                viewModel.processing(true);
                Vfs.deleteItems(this).done(function () {
                    that.parent.children.remove(that);
                    if (viewModel.selected() === this) {
                        viewModel.selected(this.parent);
                    }
                }).always(function () {
                    viewModel.processing(false);
                });
            }
        }
        this.selectNode = function () {
            var that = this;
            return this.fetchChildren().pipe(function () {
                stashCurrentSelection(viewModel.selected());
                viewModel.selected(that);

                return $.Deferred().resolve();
            });
        };
        this.selectChild = function (descendantPath) {
            var that = this;
            return this.fetchChildren().pipe(function () {
                var childName = descendantPath.split(/\/|\\/)[0],
                    matches = $.grep(that.children(), function (elm) {
                        return elm.name() === childName;
                    }),
                    deferred;
                if (matches && matches.length) {
                    var selectedChild = matches[0];
                    viewModel.selected(selectedChild);
                    if (descendantPath.length > childName.length) {
                        deferred = selectedChild.selectChild(descendantPath.substring(childName.length + 1));
                    }
                    selectedChild.fetchChildren();
                }

                return deferred || $.Deferred().resolve();
            });
        }

        this.selectParent = function () {
            var that = viewModel.selected();
            if (that.parent) {
                stashCurrentSelection(that);
                viewModel.selected(that.parent);
            }
        }
    }

    var root = new node({ name: "/", type: "dir", href: "/vfs/" }),
        viewModel = { root: root, selected: ko.observable(root), processing: ko.observable(false) },
        ignoreWorkingDirChange = true,
        workingDirChanging = false;

    root.fetchChildren();
    ko.applyBindings(viewModel, document.getElementById('#fileList'));

    window.KuduExec.workingDir.subscribe(function (newValue) {
        if (ignoreWorkingDirChange) {
            ignoreWorkingDirChange = false;
            return;
        }
        var appRoot = window.KuduExec.appRoot.toLowerCase();
        if (newValue.length >= appRoot.length && newValue.toLowerCase().indexOf(appRoot) === 0) {
            workingDirChanging = true;
            var relativeDir = newValue.substring(appRoot.length).replace(/^(\/|\\)?(.*)(\/|\\)?$/g, '$2'),
                deferred;
            if (!relativeDir) {
                deferred = viewModel.root.selectNode();
            } else {
                stashCurrentSelection(viewModel.selected());
                deferred = viewModel.root.selectChild(relativeDir);
            }
            deferred.done(function () {
                workingDirChanging = false;
            });
        }
    });

    viewModel.selected.subscribe(function (newValue) {
        if (!workingDirChanging) {
            var path = window.KuduExec.appRoot + '\\' + newValue.appRelativePath();
            // Mark it so that no-op the subscribe callback.
            ignoreWorkingDirChange = true;
            window.KuduExec.changeDir(path);
        }
    });

    function stashCurrentSelection(selected) {
        window.history.replaceState(selected.appRelativePath(), selected.name());
    }

    window.onpopstate = function (evt) {
        var selected = viewModel.selected();
        if (selected.parent) {
            viewModel.selected(selected.parent);
        }
    };

    $("#fileList").on("keydown", "input[type=text]", function (evt) {
        var context = ko.contextFor(this),
            data = context.$data;

        if (evt.which === 27) { // Cancel if Esc is pressed.
            data.parent.children.remove(data);
            return false;
        }
    });

    $("#createFolder").click(function () {
        var newFolder = new node({ name: "", type: "dir", href: "", editing: true }, viewModel.selected());
        $(this).prop("disabled", true);
        viewModel.selected().children.unshift(newFolder);
        $("#fileList input[type='text']").focus();

        newFolder.name.subscribe(function (value) {
            newFolder.href = newFolder.parent.href + '/' + value;
            newFolder.editing(false);
            Vfs.createFolder(newFolder);
            $("#createFolder").prop("disabled", false);
        });
    });

    // Drag and drop
    $('#fileList')
      .on('dragenter dragover', function (e) {
          e.preventDefault();
          e.stopPropagation();
      })
      .on('drop', function (evt) {
          evt.preventDefault();
          evt.stopPropagation();

          viewModel.processing(true);
          _getInputFiles(evt).done(function (files) {
              Vfs.addFiles(files).always(function () {
                  viewModel.selected().fetchChildren(/* force */ true);
                  viewModel.processing(false);
              });
          });
      });

    function _getInputFiles(evt) {
        var dt = evt.originalEvent.dataTransfer,
            items = evt.originalEvent.dataTransfer.items;

        if (items && items.length) {
            return whenArray($.map(items, function (item) {
                var entry = (item.webkitGetAsEntry || item.getAsEntry).apply(item);
                return _processEntry(entry);
            })).pipe(function () {
                return Array.prototype.concat.apply([], arguments);
            })
        } else {
            return $.Deferred().resolveWith(null, [$.map(dt.files, function(e) { 
                return { name: e.name, contents: e };
            })]);
        }
    }

    function _processEntry(entry, parentPath) {
        parentPath = parentPath || '';
        var deferred = $.Deferred();
        if (entry.isFile) {
            entry.file(function (file) {
                deferred.resolveWith(null, [{ name: parentPath + '/' + entry.name, contents: file }]);
            });
        } else {
            entry.createReader().readEntries(function (entries) {
                var directoryPath = parentPath + '/' + entry.name;
                whenArray($.map(entries, function (e) {
                    return _processEntry(e, directoryPath);
                })).done(function () {
                    deferred.resolveWith(null, [Array.prototype.concat.apply([], arguments)]);
                });;
            });
        }
        return deferred.promise();
    }

    function whenArray(deferreds) {
        return $.when.apply($, deferreds);
    }

    window.history.pushState("/", "/");
});