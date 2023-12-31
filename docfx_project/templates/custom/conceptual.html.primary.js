// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See LICENSE file in the project root for full license information.

var common = require('./common.js');
var extension = require('./conceptual.extension.js')

exports.transform = function (model) {
  if (extension && extension.preTransform) {
    model = extension.preTransform(model);
  }

  model._disableToc = model._disableToc || !model._tocPath || (model._navPath === model._tocPath);
  model.docurl = "https://github.com/kblok/puppeteer-sharp/issues/new?title=Improve%20" + model.source.remote.path + "&body=Explain%20how%20would%20you%20like%20this%20document%20to%20be%20impoved";

  if (extension && extension.postTransform) {
    model = extension.postTransform(model);
  }

  return model;
}