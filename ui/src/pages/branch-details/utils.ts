import type { Branch } from '../../generated/api/models';

export function namesOf(branches: Branch[]) {
	return branches.map((b) => b.name);
}
export function findBranch<T extends Branch>(
	branches: T[],
	name?: string | null,
) {
	return branches.find((b) => b.name === name);
}
