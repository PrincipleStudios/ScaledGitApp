import type { BranchConfiguration } from '@/generated/api/models';

export function getRecursiveUpstream(
	target: string,
	upstreamData: BranchConfiguration[],
) {
	const result = new Set<string>();
	const stack = [target];
	while (stack.length > 0) {
		const current = stack.pop();
		const next = upstreamData.find((e) => e.name === current)?.upstream;
		if (!next) continue;
		const upstreams = next.map((n) => n.name).filter((n) => !result.has(n));
		for (const entry of upstreams) {
			result.add(entry);
			stack.push(entry);
		}
	}
	return Array.from(result);
}
