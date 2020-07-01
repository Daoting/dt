var Uno;
(function (Uno) {
    var UI;
    (function (UI) {
        var FragmentNavigationHandler = /** @class */ (function () {
            function FragmentNavigationHandler() {
            }
            FragmentNavigationHandler.getCurrentFragment = function () {
                return window.location.hash;
            };
            FragmentNavigationHandler.setCurrentFragment = function (fragment) {
                window.location.hash = fragment;
                this.currentFragment = window.location.hash;
                return "ok";
            };
            FragmentNavigationHandler.subscribeToFragmentChanged = function () {
                var _this = this;
                if (this.subscribed) {
                    return "already subscribed";
                }
                this.subscribed = true;
                this.currentFragment = this.getCurrentFragment();
                window.addEventListener("hashchange", function (_) { return _this.notifyFragmentChanged(); }, false);
                return "ok";
            };
            FragmentNavigationHandler.notifyFragmentChanged = function () {
                var newFragment = this.getCurrentFragment();
                if (newFragment === this.currentFragment) {
                    return; // nothing to do
                }
                this.currentFragment = newFragment;
                this.initializeMethods();
                var newFragmentStr = MonoRuntime.mono_string(newFragment);
                MonoRuntime.call_method(this.notifyFragmentChangedMethod, null, [newFragmentStr]);
            };
            FragmentNavigationHandler.initializeMethods = function () {
                if (this.notifyFragmentChangedMethod) {
                    return; // already initialized.
                }
                var asm = MonoRuntime.assembly_load("Uno.Playground.WASM");
                var handlerClass = MonoRuntime.find_class(asm, "Uno.UI.Wasm", "FragmentHavigationHandler");
                this.notifyFragmentChangedMethod = MonoRuntime.find_method(handlerClass, "NotifyFragmentChanged", -1);
            };
            FragmentNavigationHandler.subscribed = false;
            return FragmentNavigationHandler;
        }());
        UI.FragmentNavigationHandler = FragmentNavigationHandler;
    })(UI = Uno.UI || (Uno.UI = {}));
})(Uno || (Uno = {}));
//# sourceMappingURL=FragmentNavigation.js.map