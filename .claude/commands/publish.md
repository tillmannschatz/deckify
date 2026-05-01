---
description: Trigger ClickOnce publish workflow — bumps version, commits, dispatches GitHub Action
allowed-tools: PowerShell, Bash, Read, Edit
argument-hint: patch | minor | major | <X.Y.Z>   (default patch)
---

Trigger a new ClickOnce release for **deckify**. Argument $ARGUMENTS:
- `patch` / `minor` / `major` — semantic bump from last release tag
- `X.Y.Z` (e.g. `0.5.0`) — explicit version
- empty → default `patch`

**Repo split:** code lives in `tillmannschatz/PPTXML` (workflow runs here, source-of-truth for `<ApplicationVersion>`). Distribution (Pages + Releases) lives in `tillmannschatz/deckify`.

## Pre-flight checks

```bash
gh secret list --repo tillmannschatz/PPTXML
gh api repos/tillmannschatz/PPTXML/actions/permissions/workflow --jq '.default_workflow_permissions'
gh api repos/tillmannschatz/deckify/pages --jq '.html_url'
```

Abort with a clear message if:
- `PFX_BASE64` or `DECKIFY_PAT` is missing from PPTXML secrets
- PPTXML workflow permissions are not `write`
- deckify Pages is not configured (HTTP 404)

## Steps

1. **Resolve current version**
   ```bash
   gh release list --repo tillmannschatz/deckify --limit 1 --json tagName --jq '.[0].tagName // "v0.0.0"'
   ```
   Strip leading `v`. Empty → `0.0.0`.

2. **Compute new version** based on `$ARGUMENTS`:
   - matches `^\d+\.\d+\.\d+$` → use as-is
   - `patch` or empty → `M.m.(p+1)`
   - `minor` → `M.(m+1).0`
   - `major` → `(M+1).0.0`
   - else → abort with usage hint

3. **Confirm with user**: `"Bump <current> → <new> — proceed? (yes/no)"`. Wait for ack.

4. **Patch csproj**: replace the `<ApplicationVersion>...</ApplicationVersion>` line in `src/Deckify/Deckify.csproj` with `<new>.0` (always 4-part, last segment `0`).

5. **Verify diff**: `git status` + `git diff src/Deckify/Deckify.csproj` — only the version line should change.

6. **Commit**:
   ```
   chore: release v<new>

   Co-Authored-By: Claude Opus 4.7 (1M context) <noreply@anthropic.com>
   ```

7. **Push**: `git push origin development` (origin = PPTXML).

8. **Trigger workflow** in PPTXML:
   ```bash
   gh workflow run publish.yml --repo tillmannschatz/PPTXML --ref development -f version=<new>
   ```

9. **Get run ID** (after ~2s):
   ```bash
   gh run list --repo tillmannschatz/PPTXML --workflow publish.yml --limit 1 --json databaseId --jq '.[0].databaseId'
   ```

10. **Watch run**:
    ```bash
    gh run watch <runId> --repo tillmannschatz/PPTXML --exit-status
    ```

11. **On success** print:
    - Release: `https://github.com/tillmannschatz/deckify/releases/tag/v<new>`
    - Pages: `https://tillmannschatz.github.io/deckify/setup.exe`

12. **On failure** print failing step name + first error line from `gh run view <runId> --repo tillmannschatz/PPTXML --log-failed`. Don't dump the full log.
