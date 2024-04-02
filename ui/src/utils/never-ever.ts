export function neverEver(something: never): never {
	throw new Error(`something ${something as string} should not have happened`);
}
