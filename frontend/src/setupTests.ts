// jest-dom adds custom jest matchers for asserting on DOM nodes.
// allows you to do things like:
// expect(element).toHaveTextContent(/react/i)
// learn more: https://github.com/testing-library/jest-dom
import '@testing-library/jest-dom/extend-expect';
import 'jest-webgl-canvas-mock';
import './test-utils/jsdom-worker';

// @ts-ignore
global.Worker.terminate = jest.fn();
global.Worker.prototype.terminate = jest.fn();

window.URL.createObjectURL = (_blob) => {
    return "";
}
window.URL.revokeObjectURL = (_blobUrl) => {
    return;
}
