window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', ev => {
    DotNet.invokeMethod('Playground.Wasm', 'OnIsDarkChanged', ev.matches);
});

function postScheme(scheme) {
    window.parent?.postMessage(scheme, '*');
}