import type { AdapterResponseArgs } from '@principlestudios/openapi-codegen-typescript';
import { parse as parseContentType } from 'content-type';

export async function responseToString(response: AdapterResponseArgs) {
	const reader = (response.response as ReadableStream).getReader();
	const contentTypeHeader = response.getResponseHeader('Content-Type');
	const textDecoder = new TextDecoder(
		(contentTypeHeader &&
			parseContentType(contentTypeHeader).parameters['charset']) ??
			'utf-8',
	);
	let result = '';

	async function read() {
		const readResponse = await reader.read();

		if (readResponse.done) {
			return result;
		}

		result += textDecoder.decode(
			readResponse.value as ArrayBuffer | ArrayBufferView,
			{ stream: true },
		);
		return read();
	}

	return read();
}
