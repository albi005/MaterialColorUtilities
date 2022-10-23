window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', ev => {
    DotNet.invokeMethod('Playground.Wasm', 'OnIsDarkChanged', ev.matches);
});