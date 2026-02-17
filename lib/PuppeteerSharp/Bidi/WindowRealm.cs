// * MIT License
//  *
//  * Copyright (c) Darío Kondratiuk
//  *
//  * Permission is hereby granted, free of charge, to any person obtaining a copy
//  * of this software and associated documentation files (the "Software"), to deal
//  * in the Software without restriction, including without limitation the rights
//  * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  * copies of the Software, and to permit persons to whom the Software is
//  * furnished to do so, subject to the following conditions:
//  *
//  * The above copyright notice and this permission notice shall be included in all
//  * copies or substantial portions of the Software.
//  *
//  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  * SOFTWARE.

#if !CDP_ONLY

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using PuppeteerSharp.Bidi.Core;
using WebDriverBiDi.Script;

namespace PuppeteerSharp.Bidi;

internal class WindowRealm(BrowsingContext browsingContext, string sandbox = null) : Core.Realm(browsingContext, string.Empty, string.Empty), IDedicatedWorkerOwnerRealm
{
    private readonly string _sandbox = sandbox;
    private readonly ConcurrentDictionary<string, DedicatedWorkerRealm> _workers = [];

    public event EventHandler<WorkerRealmEventArgs> Worker;

    public override Session Session => Context.UserContext.Browser.Session;

    public override ContextTarget Target => new(Context.Id) { Sandbox = _sandbox };

    public string ExecutionContextId { get; set; }

    public static WindowRealm From(BrowsingContext context, string sandbox = null)
    {
        var realm = new WindowRealm(context, sandbox);
        realm.Initialize();
        return realm;
    }

    public override void Dispose()
    {
        Context.Dispose();
        Session.Dispose();

        foreach (var worker in _workers.Values)
        {
            worker.Dispose();
        }

        base.Dispose();
    }

    private void Initialize()
    {
        Context.Closed += (sender, args) => Dispose(args.Reason);
        Session.Driver.Script.OnRealmCreated.AddObserver(OnWindowRealmCreated);
        Session.Driver.Script.OnRealmCreated.AddObserver(OnDedicatedRealmCreated);
    }

    private void OnWindowRealmCreated(RealmCreatedEventArgs args)
    {
        if (args.Type != RealmType.DedicatedWorker)
        {
            return;
        }

        var dedicatedWorkerInfo = args.As<DedicatedWorkerRealmInfo>();

        if (!dedicatedWorkerInfo.Owners.Contains(Id))
        {
            return;
        }

        var realm = DedicatedWorkerRealm.From(Context, this, dedicatedWorkerInfo.RealmId, dedicatedWorkerInfo.Origin);
        _workers.TryAdd(realm.Id, realm);
        realm.Destroyed += (sender, args) => _workers.TryRemove(realm.Id, out _);
        OnWorker(realm);
    }

    private void OnWorker(DedicatedWorkerRealm realm) => Worker?.Invoke(this, new WorkerRealmEventArgs(realm));

    private void OnDedicatedRealmCreated(RealmCreatedEventArgs args)
    {
        if (args.Type != RealmType.Window ||
            args.As<WindowRealmInfo>().BrowsingContext != Context.Id ||
            args.As<WindowRealmInfo>().Sandbox != _sandbox)
        {
            return;
        }

        Id = args.RealmId;
        Origin = args.Origin;
        ExecutionContextId = null;
        OnUpdated();
    }
}

#endif
